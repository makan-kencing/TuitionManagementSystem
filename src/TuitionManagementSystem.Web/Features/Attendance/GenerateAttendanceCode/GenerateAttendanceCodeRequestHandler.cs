namespace TuitionManagementSystem.Web.Features.Attendance.GenerateAttendanceCode;

using Ardalis.Result;
using System;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;

public sealed class GenerateAttendanceCodeRequestHandler(ApplicationDbContext db): IRequestHandler<GenerateAttendanceCodeRequest, Result<GenerateAttendanceCodeResponse>>
{
    public async Task <Result<GenerateAttendanceCodeResponse>> Handle(GenerateAttendanceCodeRequest request,
        CancellationToken cancellationToken)
    {

        var session = await db.Sessions
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null)
            return Result.NotFound("Session not found.");

        var random = new Random();
        bool exists;
        var sixDigitNumber=0;
            do
            {

                sixDigitNumber = random.Next(100000, 1000000);

                 exists = await db.AttendanceCodes
                    .AsNoTracking()
                    .AnyAsync(at =>
                            at.Code == sixDigitNumber.ToString() &&
                            at.Session.Id == request.SessionId &&
                            at.Session.StartAt.Date == session.StartAt.Date,
                        cancellationToken);

               if (!exists)
               {
                   var newCode = new AttendanceCode()
                   {
                       Session = session,
                       Code = sixDigitNumber.ToString(),
                       DatedFor = DateOnly.FromDateTime(DateTime.UtcNow)
                   };
                    db.AttendanceCodes.Add(newCode);
                    await db.SaveChangesAsync(cancellationToken);


                }

            } while (exists);

            return Result.Success(new GenerateAttendanceCodeResponse
            {
                Code =  sixDigitNumber.ToString(),
            });

    }
}
