namespace TuitionManagementSystem.Web.Features.Attendance.AttendanceSummary;

using Ardalis.Result;
using MediatR;

public record AttendanceSummaryRequest(int StudentId)
    : IRequest<Result<AttendanceSummaryViewModel>>;
