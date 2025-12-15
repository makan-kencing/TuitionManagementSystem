namespace TuitionManagementSystem.Web.Features.File.DeleteFile;

using Ardalis.Result;
using MediatR;

public record DeleteFileCommand(int FileId) : IRequest<Result>;
