namespace TuitionManagementSystem.Web.Features.Homework.GetSubmissionFile;

using Ardalis.Result;
using MediatR;

public record GetSubmissionFileRequest(int SubmissionId)
    : IRequest<Result<GetSubmissionFileResponse>>;

