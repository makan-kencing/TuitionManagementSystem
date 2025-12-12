namespace TuitionManagementSystem.Web.Features.Attendance.TakeAttendanceCode;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using Models.User;
using Services.Auth.Extensions;

public class TakeAttendanceCodeRequestHandler(ApplicationDbContext db ,IHttpContextAccessor httpContextAccessor)
:IRequestHandler<TakeAttendanceCodeRequest,Result<TakeAttendanceCodeResponse>>
{


    public async Task<Result<TakeAttendanceCodeResponse>> Handle(TakeAttendanceCodeRequest request,
        CancellationToken cancellationToken)
    {

        var session = await db.Sessions
            .Include(s => s.Code)
            .Include(s => s.Course)
            .Where(s => s.Code.Code == request.Code)
            .OrderByDescending(s => s.CodeGeneratedAt)
            .FirstOrDefaultAsync(cancellationToken);
        var accountId = httpContextAccessor.HttpContext!.User.GetUserId();
        var student =await db.Students
            .Where(s=> s.Account.Id == accountId)
            .FirstOrDefaultAsync(cancellationToken);
        var isEnroll =await db.Enrollments
            .Where(e=>e.Student.Id == student.Id)
            .Where(e=>e.Course.Id==session.Course.Id)
            .AnyAsync(cancellationToken);
       var existingAttendance = await db.Attendances
           .AnyAsync(a => a.Student.Id == student.Id
                       && a.Session.Id == session.Id,
                    cancellationToken);
        if (session == null)
        {
            return Result.NotFound("Attendance code not found");
        }

        if (! this.CheckWithinTheTime(session) )
        {
            return Result.Invalid();
        }

        if (isEnroll)
        {
            return Result.Invalid();
        }

        if (!existingAttendance)
        {
            return Result.Conflict("Attendance is already taken");
        }


        var attendance = new Attendance() { Session = session, Student = student, TakenOn = DateTime.UtcNow };

       await db.Attendances.AddAsync(attendance,cancellationToken);
       await db.SaveChangesAsync(cancellationToken);

        return Result<TakeAttendanceCodeResponse>.Success(new ()
        {
            SessionId = session.Id
        });
    }

    private bool CheckWithinTheTime(Session session)
    {
        var currentTime = DateTime.Now;
        return currentTime >= session.StartAt && currentTime <= session.EndAt;
    }

}
