namespace TuitionManagementSystem.Web.Features.Homework;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using File.UploadFiles;
using Microsoft.AspNetCore.Mvc;

public class MarkHomeworkViewModel
{
    public required HomeworkStudentInfo Student { get; set; }

    public required HomeworkSubmissionInfo Submission { get; set; }

    [DisplayName("Grade")]
    [Microsoft.Build.Framework.Required]
    [Range(0, 100)]
    public int? Grade { get; init; }

    public required UploadFilesResponse SubmissionFiles { get; init; }
}

public class HomeworkStudentInfo
{
    public required string Name { get; set; }
}

public class HomeworkSubmissionInfo
{
    [HiddenInput] public required int Id { get; set; }

    [DisplayName("Content")] public required string? Content { get; set; }
}
