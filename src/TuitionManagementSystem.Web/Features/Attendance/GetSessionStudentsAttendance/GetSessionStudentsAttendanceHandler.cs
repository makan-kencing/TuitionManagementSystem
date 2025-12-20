namespace TuitionManagementSystem.Web.Features.Attendance.GetSessionStudentsAttendance;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetSessionStudentsAttendanceHandler(ApplicationDbContext db)
    : IRequestHandler<GetSessionStudentsAttendanceRequest, Result<GetSessionStudentsAttendanceResponse>>
{
    public async Task<Result<GetSessionStudentsAttendanceResponse>> Handle(
        GetSessionStudentsAttendanceRequest request,
        CancellationToken cancellationToken)
    {
        var session = await db.Sessions
            .Where(s => s.Id == request.SessionId)
            .Select(s => new GetSessionStudentsAttendanceResponse
            {
                SessionId = s.Id,
                IsCodeGenerated = s.AttendanceCode != null,
                StartDate = s.StartAt,
                EndDate = s.EndAt,
                CreatedOn = DateTime.UtcNow,
                StudentList = s.Course.Enrollments
                    .GroupJoin(
                        s.Attendances,
                        e => e.Student.Id,
                        a => a.Student.Id,
                        (e, a) => new { Enrollment = e, Attendances = a }
                    ).SelectMany(
                        g => g.Attendances.DefaultIfEmpty(),
                        (g, a) => new { g.Enrollment, Attendance = a }
                    )
                    .Select(g => new StudentInfo
                        {
                            StudentId = g.Enrollment.Student.Id,
                            DisplayName = g.Enrollment.Student.Account.DisplayName,
                            AttendanceId = g.Attendance == null ? null : g.Attendance.Id
                        }
                    ).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Result.NotFound();
        }

        return Result.Success(session);
    }
}
