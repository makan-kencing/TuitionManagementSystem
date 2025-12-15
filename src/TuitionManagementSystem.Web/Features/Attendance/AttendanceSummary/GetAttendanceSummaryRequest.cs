namespace TuitionManagementSystem.Web.Features.Attendance.AttendanceSummary;

using Ardalis.Result;
using MediatR;

public record GetAttendanceSummaryRequest(int UserId)
    : IRequest<Result<GetAttendanceSummaryResponse>>;
