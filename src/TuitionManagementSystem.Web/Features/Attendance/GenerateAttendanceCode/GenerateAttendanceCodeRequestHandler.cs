namespace TuitionManagementSystem.Web.Features.Attendance.GenerateAttendanceCode;

using Ardalis.Result;
using System;
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

        LastCodeId = (LastCodeId % 1_000_000) + 1;
        var code = await db.AttendanceCodes
            .Where(at => at.Id == LastCodeId)
            .FirstAsync(cancellationToken);

        session.Code = code;
        session.CodeGeneratedAt = DateTime.UtcNow;

        return Result<GenerateAttendanceCodeResponse>.Success(new()
        {
            Code = code.Code
        });
    }
}
