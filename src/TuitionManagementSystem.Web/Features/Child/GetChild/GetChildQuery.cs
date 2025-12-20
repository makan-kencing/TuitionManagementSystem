namespace TuitionManagementSystem.Web.Features.Child.GetChild;

using Ardalis.Result;
using MediatR;

public record GetChildQuery(int UserId, int ChildId) : IRequest<Result<GetChildResponse>>;
