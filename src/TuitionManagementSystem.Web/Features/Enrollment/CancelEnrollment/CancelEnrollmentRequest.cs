namespace TuitionManagementSystem.Web.Features.Enrollment.CancelEnrollment;

using Ardalis.Result;
using MediatR;

public record CancelEnrollmentRequest(int EnrollmentId)
    : IRequest<Result>;
