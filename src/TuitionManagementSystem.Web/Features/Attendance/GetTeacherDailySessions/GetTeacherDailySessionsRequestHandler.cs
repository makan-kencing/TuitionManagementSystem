namespace TuitionManagementSystem.Web.Features.Attendance.GetTeacherDailySessions;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Schedule;

public sealed class GetTeacherDailySessionsRequestHandler(ApplicationDbContext db)
    : IRequestHandler<GetTeacherDailySessionsRequest, Result<GetTeacherDailySessionsResponse>>
{
    public async Task<Result<GetTeacherDailySessionsResponse>> Handle(GetTeacherDailySessionsRequest request,
        CancellationToken cancellationToken)
    {
        var timezone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur");

        var hour0 = DateTimeUtc.ToUtcAssumingLocal(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,timezone).Date);
        var hour24 = hour0.AddDays(1);

        var courses = await db.CourseTeachers
            .Where(ct => ct.TeacherId == request.UserId)
            .Select(ct => ct.Course)
            .Select(c => new CourseDaily
            {
                Id = c.Id,
                SubjectName = c.Subject.Name,
                Name = c.Name,
                Sessions = c.Sessions
                    .Where(s => s.StartAt >= hour0 && s.StartAt < hour24)
                    .Select(s => new SessionDaily
                    {
                        Id = s.Id,
                        StartAt = s.StartAt,
                        EndAt = s.EndAt,
                        Code = s.AttendanceCode != null
                            ? s.AttendanceCode.Code
                            : null
                    }).ToList()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(new GetTeacherDailySessionsResponse { Courses = courses });
    }
}
