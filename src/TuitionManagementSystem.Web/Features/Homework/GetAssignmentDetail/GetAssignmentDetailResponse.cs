namespace TuitionManagementSystem.Web.Features.Homework.GetAssignmentDetail;

public class GetAssignmentDetailsResponse
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

    public required ICollection<StudentHomework> Assigned { get; set; }
}

public class StudentHomework
{
    public required string? StudentName { get; set; }

    public required AssignmentSubmission? Submission { get; set; }
}

public class AssignmentSubmission
{
    public required int Id { get; set; }

    public string? Content { get; set; }

    public int? Grade { get; set; }

    public required DateTime SubmittedAt { get; set; }

    public required ICollection<AssignmentSubmissionFile> Files { get; set; }
}

public class AssignmentSubmissionFile
{
    public required string FileName { get; set; }

    public required string MimeType { get; set; }

    public required string MappedPath { get; set; }
}
