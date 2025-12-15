namespace TuitionManagementSystem.Web.Features.Enrollment.ViewEnrollment;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class ViewEnrollmentHandler
    : IRequestHandler<ViewEnrollmentRequest, Result<List<ViewEnrollmentResponse>>>
{
    private readonly ApplicationDbContext _db;

    public ViewEnrollmentHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<ViewEnrollmentResponse>>> Handle(
        ViewEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var enrollments = await _db.Enrollments
            .Where(e => e.Student.Id == request.StudentId)
            .Select(e => new ViewEnrollmentResponse
            {
                EnrollmentId = e.Id,
                StudentId = e.Student.Id,
                CourseId = e.Course.Id,
                CourseName = e.Course.Name,
                EnrolledAt = e.EnrolledAt,
            })
            .ToListAsync(cancellationToken);

        if (!enrollments.Any())
            return Result.NotFound();

        return Result.Success(enrollments);
    }
}

