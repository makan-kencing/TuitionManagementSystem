namespace TuitionManagementSystem.Web.Features.File.UploadFiles;

using System.Net.Mime;
using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.File;

public class UploadFilesCommandHandler(ApplicationDbContext db, IFileService fileService)
    : IRequestHandler<UploadFilesCommand, Result<UploadFilesResponse>>
{
    public static readonly string[] AllowedMimeTypes =
    [
        MediaTypeNames.Image.Png,
        MediaTypeNames.Image.Jpeg,
        MediaTypeNames.Image.Webp,
        MediaTypeNames.Application.Pdf,
        MediaTypeNames.Application.Xml,
        MediaTypeNames.Application.Json,
        MediaTypeNames.Application.Rtf,
        MediaTypeNames.Application.Zip,
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        MediaTypeNames.Text.Css,
        MediaTypeNames.Text.Csv,
        MediaTypeNames.Text.Plain,
        MediaTypeNames.Text.JavaScript,
        MediaTypeNames.Text.Markdown,
        MediaTypeNames.Text.RichText,
        MediaTypeNames.Text.Rtf,
        MediaTypeNames.Text.Xml
    ];

    public async Task<Result<UploadFilesResponse>> Handle(UploadFilesCommand request,
        CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Where(u => u.Id == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Unauthorized();
        }

        List<File> files = [];
        foreach (var formFile in request.Files)
        {
            if (!AllowedMimeTypes.Contains(formFile.ContentType))
            {
                return Result.Invalid(new ValidationError($"Filetype {formFile.ContentType} is not accepted."));
            }

            var saved = await fileService.UploadFileAsync(formFile);

            files.Add(new File
            {
                FileName = formFile.FileName,
                MimeType = formFile.ContentType,
                Uri = saved.MappedPath,
                CanonicalPath = saved.CanonicalPath,
                CreatedBy = user
            });
        }

        await db.AddRangeAsync(files, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var response = new UploadFilesResponse();
        response.AddRange(files.Select(f => new UploadedFile(f.Id, f.FileName, f.Uri)));

        return Result.Success(response);
    }
}
