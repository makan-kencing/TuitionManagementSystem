namespace TuitionManagementSystem.Web.Services.File;

using Microsoft.Extensions.FileProviders;

public interface IFileService
{
    public PathString MappedPath { get; }

    public PathString PhysicalPath { get; }

    public IFileProvider FileProvider { get; }

    public Task<SavedFile> UploadFileAsync(IFormFile formFile);

    public Task DeleteFileAsync(PathString canonicalPath);
}
