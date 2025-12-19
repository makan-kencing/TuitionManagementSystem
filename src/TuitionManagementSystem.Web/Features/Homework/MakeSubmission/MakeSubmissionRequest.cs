namespace TuitionManagementSystem.Web.Features.Homework.MakeSubmission;
using Ardalis.Result;
using MediatR;

public record MakeSubmissionRequest(int AssignmentId, int UserId, ICollection<int> FileIds,string Content)
    : IRequest<Result<MakeSubmissionResponse>>;

