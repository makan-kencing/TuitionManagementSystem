namespace TuitionManagementSystem.Web.Features.Attendance.TakeAttendanceCode;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using Services.Auth.Extensions;

public class TakeAttendanceCodeRequestHandler(ApplicationDbContext db ,IHttpContextAccessor httpContextAccessor)
:IRequestHandler<TakeAttendanceCodeRequest,Result<TakeAttendanceCodeResponse>>
{


    public async Task<Result<TakeAttendanceCodeResponse>> Handle(TakeAttendanceCodeRequest request,
        CancellationToken cancellationToken)
    {

        var session =await  db.Sessions
            .Where(s=> s.Code.ToString()==request.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (session == null)
        {
            return Result.NotFound("Attendance code not found");
        }

        if (! this.CheckWithinTheTime(session) || !this.CheckWithEnrollment(session))
        {
            return Result.NotFound("Attendance not take in a time");
        }

        var accountId = httpContextAccessor.HttpContext!.User.GetUserId();
        var student =await db.Students
            .Where(s=> s.Account.Id == accountId)
            .FirstOrDefaultAsync(cancellationToken);

        var attendance = new Attendance() { Session = session, Student = student, TakenOn = DateTime.UtcNow };

        var addedAttendance = await db.Attendances.AddAsync(attendance,cancellationToken);

        return Result<TakeAttendanceCodeResponse>.Success(new ()
        {
            SessionId = session.Id.ToString()
        });
    }

    private bool CheckWithinTheTime(Session session)
    {
        var currentTime = DateTime.Now;
        return currentTime >= session.StartAt && currentTime <= session.EndAt;
    }

    private bool CheckWithEnrollment(Session session)
    {
        if (session==null)
        {
            return false;
        }
        return true;
    }




}
