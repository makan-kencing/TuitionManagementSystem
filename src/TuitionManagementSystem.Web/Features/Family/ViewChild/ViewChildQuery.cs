namespace TuitionManagementSystem.Web.Features.Family.ViewChild;

using Ardalis.Result;
using MediatR;

public record ViewChildQuery(int ChildId) : IRequest<Result<ViewChildResponse>>;
