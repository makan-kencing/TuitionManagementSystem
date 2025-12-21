namespace TuitionManagementSystem.Web.Features.Dashboard.TeacherDashboard;

using MediatR;

public class TeacherDashboardRequest : IRequest<TeacherDashboardResponse>
{
    public int TeacherId { get; init; }
}
