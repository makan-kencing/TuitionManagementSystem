namespace TuitionManagementSystem.Web.Features.File;

using Abstractions;
using Ardalis.Result;
using DeleteFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UploadFiles;

public class FileController(IMediator mediator) : ApiController
{
    [HttpPost]
    public async Task<Result<UploadFilesResponse>> Upload(IFormFileCollection files) =>
        await mediator.Send(new UploadFilesCommand(files));

    [HttpDelete]
    [Route("{id:int:required}")]
    public async Task<Result> Delete(int id) =>
        await mediator.Send(new DeleteFileCommand(id));
}
