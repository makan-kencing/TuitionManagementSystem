namespace TuitionManagementSystem.Web.Features.Homework.MakeAnnouncement;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Models.Class.Announcement;
using Schedule;

public sealed class MakeAnnouncementInfoRequestHandler(ApplicationDbContext db)
    : IRequestHandler<MakeAnnouncementInfoRequest, Result<MakeAnnouncementInfoResponse>>
{
    public async Task<Result<MakeAnnouncementInfoResponse>> Handle(MakeAnnouncementInfoRequest request,
        CancellationToken cancellationToken)
    {
        var announcement = new Assignment
        {
            Title = request.Title,
            CourseId = request.CourseId,
            Description = request.Description,
            CreatedById = request.UserId,
            DueAt = DateTimeUtc.ToUtcAssumingLocal(request.DueAt),
            Attachments = request.FileIds
                .Select(id => new AnnouncementFile { FileId = id }).ToList()
        };
        await db.Announcements.AddRangeAsync(announcement);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
