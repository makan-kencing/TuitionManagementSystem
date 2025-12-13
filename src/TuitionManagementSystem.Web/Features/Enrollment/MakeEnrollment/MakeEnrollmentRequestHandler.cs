namespace TuitionManagementSystem.Web.Features.Enrollment.MakeEnrollment;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;

public class MakeEnrollmentRequestHandler(ApplicationDbContext db)
    : IRequestHandler<MakeEnrollmentRequest, Result<MakeEnrollmentResponse>>
{
    public async Task<Result<MakeEnrollmentResponse>> Handle(
        MakeEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var student = await db.Students
            .Where(s => s.Id == request.StudentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (student == null)
        {
            return Result.NotFound("Student not found");
        }

        var course = await db.Courses
            .Where(c => c.Id == request.CourseId)
            .FirstOrDefaultAsync(cancellationToken);

        if (course == null)
        {
            return Result.NotFound("Course not found");
        }

        var isEnrolled = await db.Enrollments
            .AnyAsync(e =>
                    e.Student.Id == student.Id &&
                    e.Course.Id == course.Id,
                cancellationToken);

        if (isEnrolled)
        {
            return Result.Conflict("Student is already enrolled in this course");
        }

        var enrollment = new Enrollment
        {
            Student = student,
            Course = course,
            EnrolledAt = DateTime.UtcNow
        };

        await db.Enrollments.AddAsync(enrollment, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result<MakeEnrollmentResponse>.Success(new MakeEnrollmentResponse
        {
            EnrollmentId = enrollment.Id,
            StudentId = student.Id,
            CourseId = course.Id,
            EnrolledAt = enrollment.EnrolledAt
        });
    }
}
