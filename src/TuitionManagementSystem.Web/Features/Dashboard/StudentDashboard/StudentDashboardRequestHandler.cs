namespace TuitionManagementSystem.Web.Features.Dashboard.StudentDashboard
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using TuitionManagementSystem.Web.Infrastructure.Persistence;
    using TuitionManagementSystem.Web.Models.Payment;

    public class StudentDashboardRequestHandler(ApplicationDbContext db)
        : IRequestHandler<StudentDashboardRequest, StudentDashboardResponse>
    {
        public async Task<StudentDashboardResponse> Handle(
            StudentDashboardRequest request,
            CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            // -------------------------
            // Sessions TODAY for student
            // -------------------------
            var todaySessionsQuery = db.Sessions
                .Where(s =>
                    s.StartAt >= today &&
                    s.StartAt < tomorrow &&
                    s.Course.Enrollments.Any(e =>
                        e.StudentId == request.StudentId));

            var totalSessionsToday = await todaySessionsQuery
                .CountAsync(cancellationToken);

            // -------------------------
            // Attendance TAKEN today
            // -------------------------
            var attendanceTakenToday = await db.Attendances
                .CountAsync(a =>
                        a.StudentId == request.StudentId &&
                        a.Session.StartAt >= today &&
                        a.Session.StartAt < tomorrow,
                    cancellationToken);

            // -------------------------
            // Classes Today
            // -------------------------
            var classesToday = totalSessionsToday;

            // -------------------------
            // Homework Pending
            // -------------------------
            var homeworkPending = await db.Assignments
                .Where(a =>
                    a.Course.Enrollments.Any(e =>
                        e.StudentId == request.StudentId) &&
                    !db.Submissions.Any(s =>
                        s.AssignmentId == a.Id &&
                        s.StudentId == request.StudentId))
                .CountAsync(cancellationToken);

            // -------------------------
            // Pending Amount
            // -------------------------
            var pendingAmount = await db.Invoices
                .Where(i =>
                    i.StudentId == request.StudentId &&
                    i.Status == InvoiceStatus.Pending)
                .SumAsync(i => (decimal?)i.Amount, cancellationToken) ?? 0;

            // -------------------------
            // Overdue Amount
            // -------------------------
            var overdueAmount = await db.Invoices
                .Where(i =>
                    i.StudentId == request.StudentId &&
                    i.Status == InvoiceStatus.Overdue)
                .SumAsync(i => (decimal?)i.Amount, cancellationToken) ?? 0;

            return new StudentDashboardResponse
            {
                AttendanceTakenToday = attendanceTakenToday,
                TotalSessionsToday = totalSessionsToday,
                ClassesToday = classesToday,
                HomeworkPending = homeworkPending,
                PendingAmount = pendingAmount,
                OverdueAmount = overdueAmount
            };
        }
    }
}
