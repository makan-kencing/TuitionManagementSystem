namespace TuitionManagementSystem.Web.Features.Child.GetChildrenEnrollment;

using Ardalis.Result;
using Enrollment.ViewEnrollment;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetChildrenEnrollmentQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetChildrenEnrollmentQuery, Result<Dictionary<string, List<ViewEnrollmentResponse>>>>
{
    public async Task<Result<Dictionary<string, List<ViewEnrollmentResponse>>>> Handle(
        GetChildrenEnrollmentQuery request,
        CancellationToken cancellationToken)
    {
        var family = await db.Parents
            .Where(p => p.Id == request.UserId)
            .Select(p => p.Family.Family)
            .FirstOrDefaultAsync(cancellationToken);

        if (family is null)
        {
            return Result.NotFound();
        }

        var enrollments = await db.Enrollments
            .Where(e => e.Student.Family.Id == family.Id)
            .GroupBy(e => e.Student)
            .Select(g => new
                {
                    g.Key.Account.Name,
                    Enrollments = g.Select(e => new ViewEnrollmentResponse
                    {
                        EnrollmentId = e.Id,
                        StudentId = e.Student.Id,
                        CourseId = e.Course.Id,
                        CourseName = e.Course.Name,
                        ClassroomName = e.Course.PreferredClassroom.Location,
                        Status = e.Status,
                        EnrolledAt = e.EnrolledAt,
                    }).ToList()
                }
            )
            .ToDictionaryAsync(g => g.Name, g => g.Enrollments, cancellationToken);

        return Result.Success(enrollments);
    }
}
