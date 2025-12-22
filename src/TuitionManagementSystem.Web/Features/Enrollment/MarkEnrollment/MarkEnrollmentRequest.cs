namespace TuitionManagementSystem.Web.Features.Enrollment.MarkEnrollment
{
    using Ardalis.Result;
    using MediatR;
    using Models.Class;

    public class MarkEnrollmentRequest : IRequest<Result>
    {
        public int EnrollmentId { get; set; }
        public Enrollment.EnrollmentStatus Status { get; set; }
        public string? CurrentUserType { get; set; }
    }
}
