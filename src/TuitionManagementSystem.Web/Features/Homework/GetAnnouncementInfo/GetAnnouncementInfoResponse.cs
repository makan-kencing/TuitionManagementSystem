namespace TuitionManagementSystem.Web.Features.Homework.GetAnnouncementInfo;

using File.UploadFiles;

public class GetAnnouncementInfoResponse
{
    public string? TeacherName { get; set; }

    public required CourseInfo CourseInfo { get; set; }

    public required List<AnnouncementInfo> AnnouncementInfos { get; set; }

}

public class AnnouncementInfo
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? TeacherName { get; set; }

    public required ICollection<AnnouncementFile> AttechmentFiles { get; set; }
}

public class AssignmentInfo : AnnouncementInfo
{
    public DateTime? DueAt { get; set; }
}

public class MaterialInfo : AnnouncementInfo;

public class CourseInfo
{
    public required int CourseId { get; set; }
    public required string CourseName { get; set; }

    public required string Subject { get; set; }
}

public class AnnouncementFile
{
    public required int Id { get; set; }
    public required string FileName { get; set; }

    public required string MimeType { get; set; }

    public required string MappedPath { get; set; }
}
