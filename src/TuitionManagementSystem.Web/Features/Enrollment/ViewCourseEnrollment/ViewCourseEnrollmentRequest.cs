namespace TuitionManagementSystem.Web.Features.Enrollment.ViewCourseEnrollment;

using Ardalis.Result;
using MediatR;

public record ViewCourseEnrollmentsRequest(int CourseId)
    : IRequest<Result<List<ViewCourseEnrollmentsResponse>>>;
