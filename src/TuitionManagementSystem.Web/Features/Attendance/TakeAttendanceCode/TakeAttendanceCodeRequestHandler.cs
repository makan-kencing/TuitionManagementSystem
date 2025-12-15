namespace TuitionManagementSystem.Web.Features.Attendance.TakeAttendanceCode;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using Services.Auth.Extensions;

public class TakeAttendanceCodeRequestHandler(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<TakeAttendanceCodeRequest, Result<TakeAttendanceCodeResponse>>
{
    public async Task<Result<TakeAttendanceCodeResponse>> Handle(TakeAttendanceCodeRequest request,
        CancellationToken cancellationToken)
    {
        var session = await db.Sessions
            .Include(s => s.AttendanceCode)
            .Where(s => s.AttendanceCode.Code == request.Code)
            .OrderByDescending(s => s.CodeGeneratedAt)
            .FirstOrDefaultAsync(cancellationToken);
        if (session == null)
        {
            return Result.NotFound("Session not found");
        }

        var userId = httpContextAccessor.HttpContext!.User.GetUserId();

        var student = await db.Students
            .Where(s => s.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (student == null)
        {
            return Result.NotFound("Student not found");
        }

        var isEnroll = await db.Enrollments
            .Where(e => e.Student.Id == student.Id)
            .Where(e => e.Course.Id == session.CourseId)
            .AnyAsync(cancellationToken);

        if (isEnroll)
        {
            return Result.Invalid();
        }

        var existingAttendance = await db.Attendances
            .AnyAsync(a => a.Student.Id == student.Id
                           && a.Session.Id == session.Id,
                cancellationToken);

        if (!existingAttendance)
        {
            return Result.Conflict("Attendance is already taken");
        }

        if (!this.CheckWithinTheTime(session))
        {
            return Result.Invalid();
        }


        var attendance = new Attendance { Session = session, Student = student, TakenOn = DateTime.UtcNow };

        await db.Attendances.AddAsync(attendance, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result<TakeAttendanceCodeResponse>.Success(new TakeAttendanceCodeResponse { SessionId = session.Id });
    }

    private bool CheckWithinTheTime(Session session)
    {
        var currentTime = DateTime.Now;
        return currentTime >= session.StartAt && currentTime <= session.EndAt;
    }
}
