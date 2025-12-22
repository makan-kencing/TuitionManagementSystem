namespace TuitionManagementSystem.Web.Features.File.DeleteFile;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.File;

public class DeleteFileCommandHandler(ApplicationDbContext db, IFileService fileService)
    : IRequestHandler<DeleteFileCommand, Result>
{
    public async Task<Result> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var file = await db.Files
            .Where(f => f.Id == request.FileId)
            .FirstOrDefaultAsync(cancellationToken);

        switch (file)
        {
            case null:
                return Result.NotFound();
            case { CreatedBy: null }:
            case { CreatedBy: not null } when file.CreatedBy.Id != request.UserId:
                return Result.Forbidden();
        }

        db.Files.Remove(file);
        await db.SaveChangesAsync(cancellationToken);

        if (file.CanonicalPath is not null)
        {
            try
            {
                await fileService.DeleteFileAsync(file.CanonicalPath);
            }
            catch (Exception)
            {
            }
        }

        return Result.Success();
    }
}
