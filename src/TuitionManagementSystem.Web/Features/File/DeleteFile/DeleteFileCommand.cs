namespace TuitionManagementSystem.Web.Features.File.DeleteFile;

using Ardalis.Result;
using MediatR;

public record DeleteFileCommand(int UserId, int FileId) : IRequest<Result>;
