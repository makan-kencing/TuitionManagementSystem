namespace TuitionManagementSystem.Web.Features.Attendance.TeacherDailySessionList;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetTeacherDailySessionListRequestHandler(ApplicationDbContext db)
    : IRequestHandler<GetTeacherDailySessionListRequest, Result<GetTeacherDailySessionListResponse>>
{
    public async Task<Result<GetTeacherDailySessionListResponse>> Handle(GetTeacherDailySessionListRequest request,
        CancellationToken cancellationToken)
    {
        var courses = await db.CourseTeachers
            .Where(ct => ct.TeacherId == request.UserId)
            .Select(ct => ct.Course)
            .ToListAsync(cancellationToken);

        if (courses.Count == 0)
        {
            return Result.NotFound("Course not found");
        }

        var courseNames = courses.Select(c => new CourseDaily
        {
            Course = c.Name
        }).ToList();

        var startDate = DateTime.UtcNow.Date.Subtract(TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur").BaseUtcOffset);
        var endDate = startDate.AddDays(1);


        var sessions = await db.Sessions
            .Where(s => courses.Contains(s.Course))
            .Where(s => s.StartAt >= startDate && s.StartAt < endDate)
            .Select(s => new SessionDaily
            {
                SessionId = s.Id, StartAt = s.StartAt, EndAt = s.EndAt
            })
            .OrderBy(s => s.StartAt)
            .ToListAsync(cancellationToken);



        return Result.Success(new GetTeacherDailySessionListResponse
        {
            Sessions = sessions,
            Courses =courseNames
        });
    }
}
