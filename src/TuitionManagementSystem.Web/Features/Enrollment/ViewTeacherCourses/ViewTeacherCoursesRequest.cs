namespace TuitionManagementSystem.Web.Features.Enrollment.ViewTeacherCourses;

using Ardalis.Result;
using MediatR;

public record ViewTeacherCoursesRequest(int TeacherId)
    : IRequest<Result<List<ViewTeacherCoursesResponse>>>;
