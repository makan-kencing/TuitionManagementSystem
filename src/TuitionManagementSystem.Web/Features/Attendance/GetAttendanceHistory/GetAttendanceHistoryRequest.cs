namespace TuitionManagementSystem.Web.Features.Attendance.GetAttendanceHistory;

using Ardalis.Result;
using MediatR;

public record GetAttendanceHistoryRequest(int CourseId,int UserId)
    : IRequest<Result<GetAttendanceHistoryResponse>>;
