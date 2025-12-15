namespace TuitionManagementSystem.Web.Features.Enrollment.MakeEnrollment;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using TuitionManagementSystem.Web.Features.Invoice.CreateInvoice;

public class MakeEnrollmentRequestHandler
    : IRequestHandler<MakeEnrollmentRequest, Result<MakeEnrollmentResponse>>
{
    private readonly ApplicationDbContext _db;
    private readonly IMediator _mediator;

    public MakeEnrollmentRequestHandler(
        ApplicationDbContext db,
        IMediator mediator)
    {
        _db = db;
        _mediator = mediator;
    }

    public async Task<Result<MakeEnrollmentResponse>> Handle(
        MakeEnrollmentRequest request,
        CancellationToken cancellationToken)
    {

        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken);

        if (student == null)
            return Result.NotFound("Student not found");

        var course = await _db.Courses
            .Include(c => c.PreferredClassroom)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course == null)
            return Result.NotFound("Course not found");

        var alreadyEnrolled = await _db.Enrollments.AnyAsync(e =>
            e.Student.Id == request.StudentId &&
            e.Course.Id == request.CourseId &&
            e.Status == Enrollment.EnrollmentStatus.Active,
            cancellationToken);

        if (alreadyEnrolled)
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
            Student = student!,
            Course = course!,
            Status = Enrollment.EnrollmentStatus.Active,
            EnrolledAt = DateTime.UtcNow
        };

        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync(cancellationToken);

        var invoiceResult = await _mediator.Send(
            new CreateInvoiceRequest
            {
                StudentId = request.StudentId,
                EnrollmentId = enrollment.Id
            },
            cancellationToken);

        if (!invoiceResult.IsSuccess)
            return Result.Error("Enrollment created but failed to generate invoice");

        return Result.Success(new MakeEnrollmentResponse
        {
            EnrollmentId = enrollment.Id,
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrolledAt = enrollment.EnrolledAt
        });
    }
}
