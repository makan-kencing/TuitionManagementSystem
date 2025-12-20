namespace TuitionManagementSystem.Web.Features.Attendance.GetTeacherDailySessions;

using Ardalis.Result;
using MediatR;

public record GetTeacherDailySessionsRequest(int UserId)
    : IRequest<Result<GetTeacherDailySessionsResponse>>;
