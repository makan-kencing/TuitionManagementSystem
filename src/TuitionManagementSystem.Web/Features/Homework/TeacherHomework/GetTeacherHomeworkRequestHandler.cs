namespace TuitionManagementSystem.Web.Features.Homework.TeacherHomework;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetTeacherHomeworkRequestHandler(ApplicationDbContext db)
    : IRequestHandler<GetTeacherHomeworkRequest, Result<GetTeacherHomeworkResponse>>
{
    public async Task<Result<GetTeacherHomeworkResponse>> Handle(GetTeacherHomeworkRequest request,
        CancellationToken cancellationToken)
    {
        var userName = await db.Accounts
            .Where(a => a.User.Id == request.UserId)
            .Select(a => new TeacherInfo { TeacherId = a.Id, TeacherName = a.DisplayName })
            .FirstOrDefaultAsync(cancellationToken);

        if (userName == null)
        {
            return Result.NotFound("User not found");
        }

        var courseHandler = await db.CourseTeachers
            .Where(ct => ct.TeacherId == request.UserId)
            .Select(ct => ct.Course)
            .Select(c => new CourseInfo
            {
                CourseId = c.Id,
                CourseName = c.Name,
                SubjectName = c.Subject.Name
                
            })
            .ToListAsync(cancellationToken);

        if (courseHandler == null)
        {
            return Result.NotFound("User not found");
        }

        var teacherMenu = new GetTeacherHomeworkResponse { TeacherInfo = userName, CourseInfos = courseHandler };

        return Result<GetTeacherHomeworkResponse>.Success(teacherMenu);
    }
}
