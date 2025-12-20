namespace TuitionManagementSystem.Web.Features.Homework.GetAssignmentDetail;

using Ardalis.Result;
using MediatR;

public record GetAssignmentDetailsQuery(int AssignmentId)
    : IRequest<Result<GetAssignmentDetailsResponse>>;
