namespace TuitionManagementSystem.Web.Features.Homework.StudentHomework;

using Ardalis.Result;
using MediatR;

public record GetStudentHomeworkRequest(int UserId)
: IRequest<Result<GetStudentHomeworkResponse>>;
