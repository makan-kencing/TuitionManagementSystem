namespace TuitionManagementSystem.Web.Features.Enrollment.ViewEnrollment;

using Ardalis.Result;
using MediatR;

public record ViewEnrollmentRequest(int StudentId)
    : IRequest<Result<List<ViewEnrollmentResponse>>>;
