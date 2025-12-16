namespace TuitionManagementSystem.Web.Features.Attendance.GetAttendanceCode;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetAttendanceCodeQueryHandler(ApplicationDbContext db): IRequestHandler<GetAttendanceCodeQuery, Result<GetAttendanceCodeResponse>>
{
    public async Task<Result<GetAttendanceCodeResponse>> Handle(
        GetAttendanceCodeQuery request,
        CancellationToken cancellationToken)
    {
        var response = await db.AttendanceCodes
            .Where(at => at.SessionId == request.SessionId)
            .Select(at => new GetAttendanceCodeResponse
            {
                Code = at.Code
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
        {
            return Result.NotFound();
        }

        return Result.Success(response);
    }
}
