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
        var hour0 = DateTime.UtcNow.Date.Subtract(
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Kuala_Lumpur").BaseUtcOffset);
        var hour24 = hour0.AddDays(1);

        var courses = await db.CourseTeachers
            .Where(ct => ct.TeacherId == request.UserId)
            .Select(ct => ct.Course)
            .Select(c => new CourseDaily
            {
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

        return Result.Success(new GetTeacherDailySessionListResponse { Courses = courses });
    }
}
