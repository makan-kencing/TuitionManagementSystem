namespace TuitionManagementSystem.Web.Features.Homework.GetSubmissionFile;

using File.UploadFiles;

public class GetSubmissionFileResponse
{
    public string? Name { get; set; }
    public UploadFilesResponse SubmissionFiles { get; set; }
}



