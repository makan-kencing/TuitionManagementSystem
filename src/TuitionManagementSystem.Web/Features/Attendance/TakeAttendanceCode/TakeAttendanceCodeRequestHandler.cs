namespace TuitionManagementSystem.Web.Features.Attendance.TakeAttendanceCode;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;

public class TakeAttendanceCodeRequestHandler(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<TakeAttendanceCodeRequest, Result<TakeAttendanceCodeResponse>>
{
    public async Task<Result<TakeAttendanceCodeResponse>> Handle(TakeAttendanceCodeRequest request,
        CancellationToken cancellationToken)
    {
        var attendanceCode = await db.AttendanceCodes
            .Include(at => at.Session)
            .Where(at => at.Code == request.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (attendanceCode?.Session is null)
        {
            return Result.NotFound("Code not found.");
        }

        var session = attendanceCode.Session;

        var isEnrolled = await db.Enrollments
            .Where(e => e.Student.Id == request.UserId)
            .Where(e => e.Course.Id == session.CourseId)
            .AnyAsync(cancellationToken);

        if (!isEnrolled)
        {
            return Result.Forbidden("Cannot take attendance for class not enrolled.");
        }

        var withinTime = DateTime.UtcNow >= session.StartAt && DateTime.UtcNow <= session.EndAt;

        if (!withinTime)
        {
            return Result.Forbidden("Cannot take attendance outside of class time.");
        }

        var existingAttendance = await db.Attendances
            .Where(a => a.Student.Id == request.UserId)
            .Where(a => a.Session.Id == session.Id)
            .AnyAsync(cancellationToken);

        if (existingAttendance)
        {
            return Result.Conflict("Attendance is already taken");
        }

        var attendance = new Attendance { Session = session, StudentId = request.UserId, TakenOn = DateTime.UtcNow };
        await db.Attendances.AddAsync(attendance, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return Result<TakeAttendanceCodeResponse>.Success(new TakeAttendanceCodeResponse { SessionId = attendanceCode.Id });
    }
}
