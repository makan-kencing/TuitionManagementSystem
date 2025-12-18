namespace TuitionManagementSystem.Web.Features.Homework.GetAnnouncementInfo;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class.Announcement;
using TeacherHomework;

public sealed class GetAnnouncementInfoRequestHandler(ApplicationDbContext db)
    : IRequestHandler<GetAnnouncementInfoRequest, Result<GetAnnouncementInfoResponse>>
{
    public async Task<Result<GetAnnouncementInfoResponse>> Handle(GetAnnouncementInfoRequest request,
        CancellationToken cancellationToken)
    {
        var announcements = await db.Announcements.OfType<Assignment>()
            .Where(an => an.CreatedById == request.UserId)
            .Select(an=> new AnnouncementInfo
            {
                Title =  an.Title,
                Description =  an.Description,
                CreatedAt = an.CreatedAt,
                UpdatedAt = an.UpdatedAt,
                PublishedAt = an.PublishedAt,
                DueAt = an.DueAt
            })
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        var teacherName = await db.Accounts
            .Where(a => a.User.Id == request.UserId)
            .Select(a => a.DisplayName)
            .FirstOrDefaultAsync(cancellationToken);

        var courseInfo = await db.Courses
            .Where(c => c.Id == request.CourseId)
            .Select(c=> new CourseInfo
            {
                CourseName = c.Name,
                Subject = c.Subject.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        var announcementInfomation = new GetAnnouncementInfoResponse
        {
            AnnouncementInfos = announcements, TeacherName = teacherName ,CourseInfo = courseInfo ,Now = now
        };

        return Result<GetAnnouncementInfoResponse>.Success(announcementInfomation);

    }

}
