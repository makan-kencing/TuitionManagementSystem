namespace TuitionManagementSystem.Web.Features.Child.GetChildrenEnrollment;

using Ardalis.Result;
using Enrollment.ViewEnrollment;
using MediatR;

public record GetChildrenEnrollmentQuery(int UserId) : IRequest<Result<Dictionary<string, List<ViewEnrollmentResponse>>>>;
