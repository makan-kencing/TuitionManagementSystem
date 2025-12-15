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
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken);

        if (student == null)
            return Result.NotFound("Student not found");

        var course = await db.Courses
            .Include(c => c.PreferredClassroom)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

        if (course == null)
            return Result.NotFound("Course not found");

        var isEnrolled = await db.Enrollments.AnyAsync(e =>
            e.Student.Id == student.Id &&
            e.Course.Id == course.Id,
            cancellationToken);

        if (isEnrolled)
            return Result.Conflict("Student is already enrolled in this course");

        var currentEnrollmentCount = await db.Enrollments.CountAsync(e =>
            e.Course.Id == course.Id,
            cancellationToken);

        var maxCapacity = course.PreferredClassroom.MaxCapacity;

        if (currentEnrollmentCount >= maxCapacity)
        {
            return Result.Conflict(
                $"Classroom capacity reached ({currentEnrollmentCount}/{maxCapacity})");
        }

        var enrollment = new Enrollment
        {
            Student = student,
            Course = course,
            EnrolledAt = DateTime.UtcNow
        };

        await db.Enrollments.AddAsync(enrollment, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new MakeEnrollmentResponse
        {
            EnrollmentId = enrollment.Id,
            StudentId = student.Id,
            CourseId = course.Id,
            EnrolledAt = enrollment.EnrolledAt
        });
    }
}

