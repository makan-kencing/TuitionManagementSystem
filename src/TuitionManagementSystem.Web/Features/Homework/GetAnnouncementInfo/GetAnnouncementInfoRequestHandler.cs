namespace TuitionManagementSystem.Web.Features.Homework.GetAnnouncementInfo;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class.Announcement;

// GetCourseAnnouncements
public sealed class GetAnnouncementInfoRequestHandler(ApplicationDbContext db)
    : IRequestHandler<GetAnnouncementInfoRequest, Result<GetAnnouncementInfoResponse>>
{
    public async Task<Result<GetAnnouncementInfoResponse>> Handle(GetAnnouncementInfoRequest request,
        CancellationToken cancellationToken)
    {
        var course = await db.Courses
            .Where(c => c.Id == request.CourseId)
            .Include(c => c.Announcements)
            .ThenInclude(a => a.CreatedBy.Account)
            .Select(c => new
            {
                c.Id,
                c.Name,
                SubjectName = c.Subject.Name,
                TeacherName = c.TeachersInCharge.First().Teacher.Account.DisplayName,
                c.Announcements
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            return Result.NotFound();
        }

        // GetCourseAnnouncements
        var courseAnnouncements = new GetAnnouncementInfoResponse
        {
            TeacherName = course.TeacherName,
            CourseInfo =
                new CourseInfo { CourseName = course.Name, Subject = course.SubjectName, CourseId = course.Id },
            AnnouncementInfos = course.Announcements
                .Select(announcement => announcement switch
                {
                    Assignment assignment => new AssignmentInfo
                    {
                        Id = assignment.Id,
                        Title = assignment.Title,
                        Description = assignment.Description,
                        CreatedAt = assignment.CreatedAt,
                        UpdatedAt = assignment.UpdatedAt,
                        TeacherName = assignment.CreatedBy.Account.DisplayName,
                        DueAt = assignment.DueAt
                    },
                    Material material => new MaterialInfo
                    {
                        Id = material.Id,
                        Title = material.Title,
                        Description = material.Description,
                        CreatedAt = material.CreatedAt,
                        UpdatedAt = material.UpdatedAt,
                        TeacherName = material.CreatedBy.Account.DisplayName
                    },
                    _ => new AnnouncementInfo
                    {
                        Id = announcement.Id,
                        Title = announcement.Title,
                        Description = announcement.Description,
                        CreatedAt = announcement.CreatedAt,
                        UpdatedAt = announcement.UpdatedAt,
                        TeacherName = announcement.CreatedBy.Account.DisplayName
                    }
                }).ToList()
        };


        return Result<GetAnnouncementInfoResponse>.Success(courseAnnouncements);
    }
}
