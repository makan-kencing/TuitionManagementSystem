namespace TuitionManagementSystem.Web.Features.Family.EditFamilyName;

using Ardalis.Result;
using MediatR;

public record EditFamilyNameCommand(int UserId, string Name) : IRequest<Result>;
