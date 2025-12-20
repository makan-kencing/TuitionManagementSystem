namespace TuitionManagementSystem.Web.Features.Homework.MarkHomework;

using Ardalis.Result;
using MediatR;

public record MarkHomeworkRequest(int SubmissionId , int Grade)
    : IRequest<Result<MarkHomeworkResponse>>;

