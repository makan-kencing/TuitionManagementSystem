namespace TuitionManagementSystem.Web.Features.Enrollment.ViewTeacherCourses;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;

public class ViewTeacherCoursesHandler
    : IRequestHandler<ViewTeacherCoursesRequest, Result<List<ViewTeacherCoursesResponse>>>
{
    private readonly ApplicationDbContext _db;

    public ViewTeacherCoursesHandler(ApplicationDbContext db)
    {
        this._db = db;
    }

    public async Task<Result<List<ViewTeacherCoursesResponse>>> Handle(
        ViewTeacherCoursesRequest request,
        CancellationToken cancellationToken)
    {
        var courses = await this._db.CourseTeachers
            .Where(ct => ct.TeacherId == request.TeacherId)
            .Include(ct => ct.Course)
            .ThenInclude(c => c.Subject)
            .Select(ct => new ViewTeacherCoursesResponse
            {
                CourseId = ct.CourseId,
                CourseName = ct.Course.Name,
                SubjectName = ct.Course.Subject.Name,
                TotalStudents = ct.Course.Enrollments.Count(e => e.Status == Enrollment.EnrollmentStatus.Active)
            })
            .ToListAsync(cancellationToken);

        return Result.Success(courses);
    }
}
