namespace TuitionManagementSystem.Web.Features.Dashboard.StudentDashboard;

using MediatR;

public class StudentDashboardRequest : IRequest<StudentDashboardResponse>
{
    public int StudentId { get; init; }
}
