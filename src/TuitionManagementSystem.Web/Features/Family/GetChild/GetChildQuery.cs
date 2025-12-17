namespace TuitionManagementSystem.Web.Features.Family.GetChild;

using Ardalis.Result;
using MediatR;

public record GetChildQuery(int UserId, int ChildId) : IRequest<Result<GetChildResponse>>;
