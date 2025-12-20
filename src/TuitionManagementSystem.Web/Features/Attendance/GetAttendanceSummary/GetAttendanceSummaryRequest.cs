namespace TuitionManagementSystem.Web.Features.Attendance.GetAttendanceSummary;

using Ardalis.Result;
using MediatR;

public record GetAttendanceSummaryRequest(int UserId)
    : IRequest<Result<GetAttendanceSummaryResponse>>;
