namespace TuitionManagementSystem.Web.Features.Family.ViewFamily;

using Ardalis.Result;
using MediatR;

public record ViewFamilyQuery : IRequest<Result<ViewFamilyResponse>>;
