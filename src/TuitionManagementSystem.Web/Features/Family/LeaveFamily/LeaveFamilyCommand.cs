namespace TuitionManagementSystem.Web.Features.Family.LeaveFamily;

using Ardalis.Result;
using MediatR;

public record LeaveFamilyCommand(int UserId) : IRequest<Result>;
