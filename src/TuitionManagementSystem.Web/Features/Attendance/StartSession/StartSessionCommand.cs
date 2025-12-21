namespace TuitionManagementSystem.Web.Features.Attendance.StartSession;

using Ardalis.Result;
using MediatR;

public record StartSessionCommand(int CourseId, DateTime StartAt, DateTime EndAt) : IRequest<Result>;
