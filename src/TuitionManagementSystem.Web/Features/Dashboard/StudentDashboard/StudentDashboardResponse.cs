namespace TuitionManagementSystem.Web.Features.Dashboard.StudentDashboard;

public class StudentDashboardResponse
{
    public int AttendanceTakenToday { get; set; }
    public int TotalSessionsToday { get; set; }

    public int ClassesToday { get; set; }

    public int HomeworkPending { get; set; }

    public decimal PendingAmount { get; set; }
    public decimal OverdueAmount { get; set; }
}
