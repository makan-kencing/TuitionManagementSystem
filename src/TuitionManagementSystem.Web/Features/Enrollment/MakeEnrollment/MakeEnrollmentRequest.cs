using Ardalis.Result;
using MediatR;

namespace TuitionManagementSystem.Web.Features.Enrollment.MakeEnrollment;

public sealed record MakeEnrollmentRequest(
    int StudentId,
    int CourseId
) : IRequest<Result<MakeEnrollmentResponse>>;
