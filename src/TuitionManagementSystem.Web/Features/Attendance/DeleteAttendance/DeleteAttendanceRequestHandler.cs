namespace TuitionManagementSystem.Web.Features.Attendance.DeleteAttendance;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteAttendanceRequestHandler(ApplicationDbContext db) :
    IRequestHandler<DeleteAttendanceRequest, Result>
{
    public async Task<Result> Handle(
        DeleteAttendanceRequest request, CancellationToken cancellationToken)
    {
        var checkAttendance = await db.Attendances
            .Where(ad => ad.Id == request.AttendanceId)
            .FirstOrDefaultAsync(cancellationToken);

        if (checkAttendance==null)
        {
            return Result.Invalid();
        }

        db.Attendances.Remove(checkAttendance);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
