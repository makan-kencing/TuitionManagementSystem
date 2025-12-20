namespace TuitionManagementSystem.Web.Features.File.UploadFiles;

using System.Collections.ObjectModel;

public class UploadFilesResponse(List<UploadedFile> l) : ReadOnlyCollection<UploadedFile>(l);

public record UploadedFile(
    int Id,
    string Filename,
    string MimeType,
    string MappedPath);
