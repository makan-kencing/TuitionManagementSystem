namespace TuitionManagementSystem.Web.Features.Dashboard.GetParentDashboard;

public class GetParentDashboardResponse
{
    public required int TotalChildren { get; init; }

    public required int PendingClasses { get; init; }

    public required int PendingHomework { get; init; }

    public required PendingInvoiceDetails PendingInvoices { get; init; }

    public required Dictionary<string, int> ChildrenAttendanceRate { get; init; }

    public required Dictionary<string, int> ChildrenSubmissionRate { get; init; }
}

public class PendingInvoiceDetails
{
    public required decimal Total { get; init; }

    public required int Count { get; init; }
}
