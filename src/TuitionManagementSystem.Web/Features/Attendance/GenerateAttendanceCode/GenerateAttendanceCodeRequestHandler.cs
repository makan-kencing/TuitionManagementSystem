namespace TuitionManagementSystem.Web.Features.Attendance.GenerateAttendanceCode;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GenerateAttendanceCodeRequestHandler(ApplicationDbContext db): IRequestHandler<GenerateAttendanceCodeRequest, Result<GenerateAttendanceCodeResponse>>
{
    private static int LastCodeId { get; set; }

    public async Task <Result<GenerateAttendanceCodeResponse>> Handle(GenerateAttendanceCodeRequest request,
        CancellationToken cancellationToken)
    {
        var session = await db.Sessions
            .Where(s => s.Id == request.SessionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (session == null)
        {
            return Result.NotFound("Session not found.");
        }


        var withinTime = DateTime.UtcNow >= session.StartAt && DateTime.UtcNow <= session.EndAt;

        if (!withinTime)
        {
            return Result.Forbidden("Cannot generate attendance outside of class time." + DateTime.UtcNow);
        }

        LastCodeId = (LastCodeId % 1_000_000) + 1;
        var code = await db.AttendanceCodes
            .Where(at => at.Id == LastCodeId)
            .FirstAsync(cancellationToken);

        code.Session = session;

        await db.SaveChangesAsync(cancellationToken);

        return Result<GenerateAttendanceCodeResponse>.Success(new()
        {
            Code = code.Code
        });
    }
}
