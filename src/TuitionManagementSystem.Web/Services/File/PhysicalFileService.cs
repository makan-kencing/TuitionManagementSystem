namespace TuitionManagementSystem.Web.Services.File;

using System.Globalization;
using Microsoft.Extensions.FileProviders;
using File = System.IO.File;

public class PhysicalFileService : IFileService
{
    public PathString MappedPath { get; } = "/uploads";

    public PathString PhysicalPath { get; }

    public IFileProvider FileProvider { get; }

    public PhysicalFileService(IWebHostEnvironment env)
    {
        this.PhysicalPath = Path.Combine(env.ContentRootPath, "assets", "uploads");

        this.FileProvider = new PhysicalFileProvider(this.PhysicalPath);
    }

    public async Task<SavedFile> UploadFileAsync(IFormFile formFile)
    {
        var dayPrefix = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var filename = $"{Guid.NewGuid()}.{Path.GetExtension(formFile.Name)}";

        var mappedPath = Path.Combine(this.MappedPath, dayPrefix, filename);
        var canonicalPath = Path.Combine(this.PhysicalPath, dayPrefix, filename);
        Directory.CreateDirectory(Path.Combine(this.PhysicalPath, dayPrefix));

        await using (var stream = File.Create(canonicalPath))
        {
            await formFile.CopyToAsync(stream);
        }

        return new SavedFile(
            mappedPath,
            canonicalPath);
    }

    public Task DeleteFileAsync(PathString canonicalPath)
    {
        File.Delete(canonicalPath);
        return Task.CompletedTask;
    }
}
