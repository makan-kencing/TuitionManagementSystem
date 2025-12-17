namespace TuitionManagementSystem.Web.Features.Attendance.DeleteAttendance;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteAttendanceRequestHandler(ApplicationDbContext db) :
    IRequestHandler<DeleteAttendanceRequest, Result<DeleteAttendanceResponse>>
{
    public async Task<Result<DeleteAttendanceResponse>> Handle(
        DeleteAttendanceRequest request, CancellationToken cancellationToken)
    {


        var checkAttendance = await db.Attendances
            .Where(ad => ad.Id == request.AttendanceId)
            .FirstOrDefaultAsync(cancellationToken);

        if (checkAttendance==null)
        {
            return Result.Invalid();
        }

        var sessionTime=await db.Sessions
            .Where(s => s.Id == request.SessionId)
            .Select(s => new SessionTime
            {
                StartAt = s.StartAt,
                EndAt = s.EndAt,
            })
            .FirstOrDefaultAsync(cancellationToken);

        var now = DateTime.UtcNow;

        var response = new DeleteAttendanceResponse
        {
            SessionId = request.SessionId, UserId = request.UserId, SessionTime = sessionTime,CurrentAt = now
        };

        db.Attendances.Remove(checkAttendance);
        await db.SaveChangesAsync(cancellationToken);

        return Result<DeleteAttendanceResponse>.Success(response);


    }
}
