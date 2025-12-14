namespace TuitionManagementSystem.Web.Features.File;

using Abstractions;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UploadFiles;

public class FileApiController(IMediator mediator) : ApiController
{
    [HttpPost]
    public async Task<Result<UploadFilesResponse>> Upload(IFormFileCollection files) =>
        await mediator.Send(new UploadFilesCommand(files));
}
