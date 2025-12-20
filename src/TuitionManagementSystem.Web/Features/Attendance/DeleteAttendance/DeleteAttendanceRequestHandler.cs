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
        var attendance = await db.Attendances
            .Include(a => a.Session)
            .Where(a => a.Id == request.AttendanceId)
            .FirstOrDefaultAsync(cancellationToken);

        if (attendance is null)
        {
            return Result.NotFound();
        }

        db.Attendances.Remove(attendance);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteAttendanceResponse
        {
            StudentId = attendance.StudentId,
            SessionId = attendance.SessionId,
            SessionTime = new SessionTime
            {
                StartAt = attendance.Session.StartAt,
                EndAt = attendance.Session.EndAt
            }
        });
    }
}
