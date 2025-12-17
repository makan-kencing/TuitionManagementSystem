namespace TuitionManagementSystem.Web.Features.Family.DeleteFamily;

using Ardalis.Result;
using MediatR;

public record DeleteFamilyCommand(int UserId) : IRequest<Result>;
