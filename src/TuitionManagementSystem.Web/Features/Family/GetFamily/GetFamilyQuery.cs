namespace TuitionManagementSystem.Web.Features.Family.GetFamily;

using Ardalis.Result;
using MediatR;

public record GetFamilyQuery : IRequest<Result<GetFamilyResponse>>;
