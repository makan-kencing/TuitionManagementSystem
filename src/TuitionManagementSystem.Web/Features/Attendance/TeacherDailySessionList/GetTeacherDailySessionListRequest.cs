namespace TuitionManagementSystem.Web.Features.Attendance.TeacherDailySessionList;

using Ardalis.Result;
using MediatR;

public record GetTeacherDailySessionListRequest(int UserId)
    : IRequest<Result<GetTeacherDailySessionListResponse>>;
