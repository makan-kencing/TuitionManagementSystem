namespace TuitionManagementSystem.Web.Features.Enrollment.MakeEnrollment;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using TuitionManagementSystem.Web.Features.Invoice.CreateInvoice;
using TuitionManagementSystem.Web.Services.Email;
using TuitionManagementSystem.Web.Models.Notification;
using System.Net.Mail;

public class MakeEnrollmentRequestHandler
    : IRequestHandler<MakeEnrollmentRequest, Result<MakeEnrollmentResponse>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMediator _mediator;
    private readonly IEmailService _emailService;

    public MakeEnrollmentRequestHandler(
        ApplicationDbContext db,
        IMediator mediator,
        IEmailService emailService)
    {
        _db = db;
        _mediator = mediator;
        _emailService = emailService;
    }

    public async Task<Result<MakeEnrollmentResponse>> Handle(
        MakeEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var student = await _db.Students
            .Include(s => s.Account)
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken);

        if (student == null)
            return Result.NotFound("Student not found");

        var course = await _db.Courses
            .Include(c => c.PreferredClassroom)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course == null)
            return Result.NotFound("Course not found");

        var alreadyActiveEnrolled = await _db.Enrollments.AnyAsync(e =>
                e.Student.Id == request.StudentId &&
                e.Course.Id == request.CourseId &&
                e.Status == Enrollment.EnrollmentStatus.Active,
            cancellationToken);

        if (alreadyActiveEnrolled)
            return Result.Conflict("Student is already actively enrolled in this course");

        var activeEnrollmentCount = await _db.Enrollments.CountAsync(e =>
                e.Course.Id == request.CourseId &&
                e.Status == Enrollment.EnrollmentStatus.Active,
            cancellationToken);

        var maxCapacity = course.PreferredClassroom.MaxCapacity;

        if (activeEnrollmentCount >= maxCapacity)
        {
            return Result.Conflict(
                $"Classroom capacity reached ({activeEnrollmentCount}/{maxCapacity})");
        }

        var enrollment = new Enrollment
        {
            Student = student,
            Course = course,
            Status = Enrollment.EnrollmentStatus.Active,
            EnrolledAt = DateTime.UtcNow
        };

        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync(cancellationToken);

        var invoiceResult = await _mediator.Send(
            new CreateInvoiceRequest { StudentId = request.StudentId, EnrollmentId = enrollment.Id },
            cancellationToken);

        if (!invoiceResult.IsSuccess)
            return Result.Error("Enrollment created but failed to generate invoice");

        var response = new MakeEnrollmentResponse
        {
            EnrollmentId = enrollment.Id,
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrolledAt = enrollment.EnrolledAt,
            StudentEmail = student.Account?.Email
        };

        if (!string.IsNullOrEmpty(student.Account?.Email))
        {
            try
            {
                var mail = new MailMessage
                {
                    Subject = "🎓 Enrollment Confirmation - Horizon Tuition Centre", IsBodyHtml = true
                };
                mail.To.Add(student.Account.Email);

                mail.Body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height:1.6; color:#333;'>
                    <div style='max-width:600px; margin:auto; padding:20px; border:1px solid #ddd; border-radius:10px;'>
                        <h2 style='color:#0d6efd;'>Enrollment Confirmation</h2>
                        <p>Dear <strong>{student.Account.Name}</strong>,</p>
                        <p>We are delighted to confirm your enrollment in the following course:</p>

                        <table style='width:100%; border-collapse:collapse; margin:20px 0;'>
                            <tr>
                                <td style='padding:8px; border:1px solid #ddd;'><strong>Course Name</strong></td>
                                <td style='padding:8px; border:1px solid #ddd;'>{course.Name}</td>
                            </tr>
                            <tr>
                                <td style='padding:8px; border:1px solid #ddd;'><strong>Enrollment Date</strong></td>
                                <td style='padding:8px; border:1px solid #ddd;'>{enrollment.EnrolledAt:dd MMM yyyy}</td>
                            </tr>
                            <tr>
                                <td style='padding:8px; border:1px solid #ddd;'><strong>Status</strong></td>
                                <td style='padding:8px; border:1px solid #ddd;'>Active</td>
                            </tr>
                        </table>

                        <p>To view your enrollment or manage your courses, click the button below:</p>
                        <p style='text-align:center;'>
                            <a href='https://localhost:8081/enrollment'
                               style='background-color:#0d6efd; color:#fff; text-decoration:none; padding:10px 20px; border-radius:5px;'>
                                View Enrollment
                            </a>
                        </p>

                        <p>If you have any questions, feel free to contact our support team.</p>
                        <p>Best regards,<br/><strong>Horizon Tuition Centre</strong></p>
                    </div>
                </body>
                </html>";

                await _emailService.SendAsync(mail, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }

        try
        {
            var notification = new Notification
            {
                UserId = student.Id,
                Message = $"You have successfully enrolled in course {course.Name}.",
                ActionUrl = $"/enrollment"
            };
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create notification: {ex.Message}");
        }

        return Result.Success(response);
    }
}
