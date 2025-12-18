namespace TuitionManagementSystem.Web.Features.Homework.TeacherHomework;

using Ardalis.Result;
using MediatR;

public record GetTeacherHomeworkRequest(int UserId)
: IRequest<Result<GetTeacherHomeworkResponse>>;
