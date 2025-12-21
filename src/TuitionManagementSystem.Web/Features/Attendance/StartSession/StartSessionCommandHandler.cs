namespace TuitionManagementSystem.Web.Features.Attendance.StartSession;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using Schedule;

public class StartSessionCommandHandler(ApplicationDbContext db) : IRequestHandler<StartSessionCommand, Result>
{
    public async Task<Result> Handle(StartSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await db.Courses
            .Where(c => c.Id == request.CourseId)
            .Select(c => new Session
            {
                CourseId = request.CourseId,
                StartAt = DateTimeUtc.ToUtcAssumingLocal(request.StartAt),
                EndAt = DateTimeUtc.ToUtcAssumingLocal(request.EndAt),
                ClassroomId = c.PreferredClassroomId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Result.NotFound();
        }

        db.Sessions.Add(session);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
