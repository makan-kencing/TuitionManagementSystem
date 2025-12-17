namespace TuitionManagementSystem.Web.Features.Family.CreateFamily;

using Ardalis.Result;
using MediatR;

public record CreateFamilyCommand(int UserId) : IRequest<Result>;
