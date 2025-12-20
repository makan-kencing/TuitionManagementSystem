namespace TuitionManagementSystem.Web.Features.File;

using Abstractions;
using Ardalis.Result;
using DeleteFile;
using Htmx;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Extensions;
using UploadFiles;

public class FileController(IMediator mediator) : WebController
{
    [HttpPost]
    [Route("~/file/upload")]
    public async Task<IActionResult> Upload(IFormFileCollection files)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        var response = await mediator.Send(new UploadFilesCommand(this.User.GetUserId(), files));

        return this.PartialView("_File", response.Value);
    }

    [HttpDelete]
    [Route("~/file/{id:int:required}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        var response = await mediator.Send(new DeleteFileCommand(this.User.GetUserId(), id));

        if (response.IsNotFound())
        {
            return this.PartialView("_DeleteFileNotFound");
        }

        if (response.IsForbidden())
        {
            return this.PartialView("_NoPermissionToDelete");
        }

        return this.PartialView("_FileDeleted", new FileDeletedViewModel { FileId = id });
    }

    public class FileDeletedViewModel
    {
        public int FileId { get; init; }
    }
}
