namespace TuitionManagementSystem.Web.Features.File.UploadFiles;

using Ardalis.Result;
using MediatR;

public record UploadFilesCommand(IFormFileCollection Files) : IRequest<Result<UploadFilesResponse>>;
