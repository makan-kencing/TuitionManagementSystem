namespace TuitionManagementSystem.Web.Features.Family.GetChild;

using Ardalis.Result;
using MediatR;

public record GetChildQuery(int ChildId) : IRequest<Result<GetChildResponse>>;
