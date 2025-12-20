namespace TuitionManagementSystem.Web.Features.File.DeleteFile;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteFileCommandHandler(ApplicationDbContext db) : IRequestHandler<DeleteFileCommand, Result>
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

        return Result.Success();
    }
}
