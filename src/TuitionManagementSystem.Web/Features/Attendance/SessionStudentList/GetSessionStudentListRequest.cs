namespace TuitionManagementSystem.Web.Features.Attendance.SessionStudentList;

using Ardalis.Result;
using MediatR;

public record GetSessionStudentListRequest(int SessionId)
    : IRequest<Result<GetSessionStudentListResponse>>;
