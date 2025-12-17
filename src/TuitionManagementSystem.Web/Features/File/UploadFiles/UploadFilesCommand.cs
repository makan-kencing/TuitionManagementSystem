namespace TuitionManagementSystem.Web.Features.File.UploadFiles;

using Ardalis.Result;
using MediatR;

public record UploadFilesCommand(int UserId, IFormFileCollection Files) : IRequest<Result<UploadFilesResponse>>;
