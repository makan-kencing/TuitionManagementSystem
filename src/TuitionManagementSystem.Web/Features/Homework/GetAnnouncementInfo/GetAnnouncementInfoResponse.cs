namespace TuitionManagementSystem.Web.Features.Homework.GetAnnouncementInfo;

public class GetAnnouncementInfoResponse
{
    public required string TeacherName { get; set; }

    public required CourseInfo CourseInfo { get; set; }
  public required List<AnnouncementInfo> AnnouncementInfos { get; set; }
  public DateTime Now { get; set; }
}

public class AnnouncementInfo
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime? DueAt { get; set; }

}

public class CourseInfo
{
    public string? CourseName { get; set; }
    public string? Subject { get; set; }
}


