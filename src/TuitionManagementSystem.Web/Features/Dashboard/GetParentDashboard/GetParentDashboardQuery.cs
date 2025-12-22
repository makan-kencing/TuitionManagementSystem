namespace TuitionManagementSystem.Web.Features.Dashboard.GetParentDashboard;

using Ardalis.Result;
using MediatR;

public record GetParentDashboardQuery(int UserId) : IRequest<Result<GetParentDashboardResponse>>;
