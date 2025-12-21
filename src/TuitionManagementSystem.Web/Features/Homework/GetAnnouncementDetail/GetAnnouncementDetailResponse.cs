namespace TuitionManagementSystem.Web.Features.Homework.GetAnnouncementDetail;

using File.UploadFiles;
using Models.Class.Announcement;

public class GetAnnouncementDetailResponse
{
    public required int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required DateTime UpdatedAt { get; set; }

    public required DateTime? DueAt { get; set; }

    public int TotalStudents { get; set; }

    public int SubmittedCount{ get; set; }

    public double AverageSubmissionRate { get; set; }

    public required StudentHomework? Assigned { get; set; }

    public UploadFilesResponse AnnouncementFiles { get; set; }

}


public class StudentHomework
{
    public required AssignmentSubmission? Submission { get; set; }
}

public class AssignmentSubmission
{
    public  int? Id { get; set; }

    public string? Content { get; set; }

    public int? Grade { get; set; }

    public required DateTime SubmittedAt { get; set; }

    public required ICollection<AssignmentSubmissionFile> Files { get; set; }
}

public class AssignmentSubmissionFile
{
    public required int Id { get; set; }
    public required string FileName { get; set; }

    public required string MimeType { get; set; }

    public required string MappedPath { get; set; }
}
