namespace TuitionManagementSystem.Web.Services.File;

using Microsoft.Extensions.FileProviders;

public interface IFileService
{
    public string MappedPath { get; }

    public string PhysicalPath { get; }

    public IFileProvider FileProvider { get; }

    public Task<SavedFile> UploadFileAsync(IFormFile formFile);

    public Task DeleteFileAsync(string canonicalPath);
}
