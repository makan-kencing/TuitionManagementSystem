namespace TuitionManagementSystem.Web.Features.File.UploadFiles;

public class UploadFilesResponse : List<UploadedFile>;

public record UploadedFile(
    int Id,
    Uri MappedPath);
