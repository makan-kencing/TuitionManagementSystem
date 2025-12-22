namespace TuitionManagementSystem.Web.Services
{
    using System.Net.Mail;
    using Ardalis.Result;
    using Email;
    using Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using MediatR;
    using Models.Notification;
    using Models.Payment;
    using TuitionManagementSystem.Web.Features.Invoice.CreateInvoice;

    public interface IInvoiceService
    {
        Task<Result<Invoice>> UpdateInvoiceStatusAsync(int invoiceId, InvoiceStatus newStatus,
            CancellationToken cancellationToken);

        Task CheckAndCreateOverdueInvoicesAsync(IMediator mediator, CancellationToken cancellationToken);
        Task GenerateMonthlyInvoicesAsync(IMediator mediator, CancellationToken cancellationToken);

        Task<Result> MarkInvoicesAsPaidAsync(IReadOnlyCollection<int> invoiceIds, int paymentId,
            CancellationToken cancellationToken);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<InvoiceService> _logger;
        private readonly IEmailService _emailService;

        public InvoiceService(ApplicationDbContext db, ILogger<InvoiceService> logger, IEmailService emailService)
        {
            _db = db;
            _logger = logger;
            _emailService = emailService;
        }


        public async Task<Result<Invoice>> UpdateInvoiceStatusAsync(
            int invoiceId,
            InvoiceStatus newStatus,
            CancellationToken cancellationToken)
        {
            var invoice = await _db.Invoices
                .Include(i => i.Payment)
                .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);

            if (invoice == null)
            {
                _logger.LogWarning("Invoice {InvoiceId} not found for status update", invoiceId);
                return Result.NotFound("Invoice not found");
            }

            var currentStatus = GetCurrentStatus(invoice);

            var validationResult = ValidateStatusTransition(currentStatus, newStatus);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Invalid status transition for invoice {InvoiceId}: {CurrentStatus} -> {NewStatus}. Error: {Error}",
                    invoiceId, currentStatus, newStatus, validationResult.ErrorMessage);
                return Result.Error(validationResult.ErrorMessage);
            }

            switch (newStatus)
            {
                case InvoiceStatus.Cancelled:
                case InvoiceStatus.Withdrawn:
                    invoice.CancelledAt = DateTime.UtcNow;
                    invoice.Status = newStatus;
                    _logger.LogInformation("Invoice {InvoiceId} marked as {Status}", invoiceId, newStatus);
                    break;

                case InvoiceStatus.Paid:
                    _logger.LogWarning("Attempted to mark invoice {InvoiceId} as paid through UpdateInvoiceStatus",
                        invoiceId);
                    return Result.Error("Use payment endpoint to mark invoice as paid");

                case InvoiceStatus.Overdue:
                    _logger.LogWarning("Attempted to manually set invoice {InvoiceId} to overdue status", invoiceId);
                    return Result.Error("Overdue invoices are created automatically when due date passes");

                case InvoiceStatus.Pending:
                    invoice.CancelledAt = null;
                    invoice.Status = newStatus;
                    _logger.LogInformation("Invoice {InvoiceId} reset to pending status", invoiceId);
                    break;
            }

            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success(invoice);
        }

        public async Task CheckAndCreateOverdueInvoicesAsync(
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting overdue invoice check");

            var now = DateTime.UtcNow;

            var overduePendingInvoices = await _db.Invoices
                .Include(i => i.Payment)
                .Where(i => i.Status == InvoiceStatus.Pending &&
                            i.DueAt.HasValue &&
                            i.DueAt.Value < now &&
                            !i.PaymentId.HasValue &&
                            !i.CancelledAt.HasValue)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Found {Count} pending invoices that are overdue",
                overduePendingInvoices.Count);

            foreach (var invoice in overduePendingInvoices)
            {
                try
                {
                    var existingOverdueInvoice = await _db.Invoices
                        .FirstOrDefaultAsync(i =>
                                i.EnrollmentId == invoice.EnrollmentId &&
                                i.InvoicedAt.Date == invoice.InvoicedAt.Date &&
                                i.DueAt == invoice.DueAt &&
                                i.Status == InvoiceStatus.Overdue &&
                                i.CancelledAt == null,
                            cancellationToken);

                    if (existingOverdueInvoice != null)
                        continue;

                    await mediator.Send(
                        new CreateInvoiceRequest { OverdueFromInvoiceId = invoice.Id },
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error creating overdue invoice from invoice {InvoiceId}",
                        invoice.Id);
                }
            }

            var overdueInvoices = await _db.Invoices
                .Include(i => i.Enrollment)
                .ThenInclude(e => e.Student)
                .ThenInclude(s => s.Account)
                .Where(i =>
                    i.Status == InvoiceStatus.Overdue &&
                    i.CancelledAt == null &&
                    i.DueAt < DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            var groupedByStudent = overdueInvoices
                .GroupBy(i => i.Enrollment.Student);

            foreach (var group in groupedByStudent)
            {
                var student = group.Key;
                var account = student.Account;

                if (string.IsNullOrWhiteSpace(account?.Email))
                    continue;

                try
                {
                    var rows = string.Join("", group.Select(i => $@"
                        <tr>
                            <td style='border:1px solid #ddd;padding:8px;'>{i.InvoicedAt:dd MMM yyyy}</td>
                            <td style='border:1px solid #ddd;padding:8px;'>{i.DueAt:dd MMM yyyy}</td>
                            <td style='border:1px solid #ddd;padding:8px;'>RM {i.Amount:F2}</td>
                            <td style='border:1px solid #ddd;padding:8px;color:#dc3545;font-weight:bold;'>Overdue</td>
                        </tr>
                    "));

                    var mail = new MailMessage
                    {
                        Subject = "⚠️ Overdue Invoice Summary - Horizon Tuition Center", IsBodyHtml = true
                    };
                    mail.To.Add(account.Email);

                    mail.Body = $@"
                    <html>
                    <body style='font-family:Arial,sans-serif;'>
                        <div style='max-width:700px;margin:auto;border:1px solid #ddd;padding:20px;border-radius:10px;'>
                            <h2 style='color:#dc3545;'>Overdue Invoice Summary</h2>

                            <p>Dear <strong>{account.Name}</strong>,</p>

                            <p>The following invoices are currently overdue:</p>

                            <table style='width:100%;border-collapse:collapse;'>
                                <thead>
                                    <tr style='background:#f8f9fa;'>
                                        <th style='border:1px solid #ddd;padding:8px;'>Invoice Date</th>
                                        <th style='border:1px solid #ddd;padding:8px;'>Due Date</th>
                                        <th style='border:1px solid #ddd;padding:8px;'>Amount</th>
                                        <th style='border:1px solid #ddd;padding:8px;'>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {rows}
                                </tbody>
                            </table>

                            <p style='margin-top:20px;'>
                                Please settle your outstanding invoices as soon as possible.
                            </p>

                            <p style='text-align:center;margin-top:25px;'>
                                <a href='https://localhost:8081/invoice/list'
                                   style='background:#dc3545;color:#fff;padding:10px 20px;border-radius:5px;text-decoration:none;'>
                                    View Invoices
                                </a>
                            </p>

                            <p>Thank you,<br/><strong>Horizon Tuition Center</strong></p>
                        </div>
                    </body>
                    </html>";

                    await _emailService.SendAsync(mail, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to send overdue invoice summary to student {StudentId}",
                        student.Id);
                }
            }

            _logger.LogInformation("Overdue invoice check completed");
        }

         public async Task GenerateMonthlyInvoicesAsync(
            IMediator mediator,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting monthly invoice generation check");

            var now = DateTime.UtcNow;

            var activeEnrollments = await _db.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.Status == Models.Class.Enrollment.EnrollmentStatus.Active)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Found {Count} active enrollments", activeEnrollments.Count);

            int createdCount = 0;
            int skippedCount = 0;
            int errorCount = 0;

            foreach (var enrollment in activeEnrollments)
            {
                try
                {
                    var latestInvoice = await _db.Invoices
                        .Where(i => i.EnrollmentId == enrollment.Id &&
                                    i.CancelledAt == null)
                        .OrderByDescending(i => i.InvoicedAt)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (latestInvoice != null)
                    {
                        var daysSinceLastInvoice = (now - latestInvoice.InvoicedAt).TotalDays;

                        if (daysSinceLastInvoice < 30)
                        {
                            _logger.LogDebug(
                                "Enrollment {EnrollmentId}: Last invoice from {InvoiceDate} ({Days} days ago), needs {RemainingDays} more days",
                                enrollment.Id, latestInvoice.InvoicedAt, Math.Floor(daysSinceLastInvoice),
                                Math.Ceiling(30 - daysSinceLastInvoice));
                            skippedCount++;
                            continue;
                        }
                    }
                    else if ((now - enrollment.EnrolledAt).TotalDays < 30)
                    {
                        _logger.LogDebug(
                            "Enrollment {EnrollmentId}: New enrollment from {EnrolledAt}, needs first invoice",
                            enrollment.Id, enrollment.EnrolledAt);
                    }

                    _logger.LogInformation(
                        "Creating invoice for enrollment {EnrollmentId} (Student: {StudentId}, Course: {CourseName})",
                        enrollment.Id, enrollment.Student.Id, enrollment.Course.Name);

                    var result = await mediator.Send(
                        new CreateInvoiceRequest { StudentId = enrollment.Student.Id, EnrollmentId = enrollment.Id },
                        cancellationToken);

                    if (result.IsSuccess)
                    {
                        _logger.LogInformation("Successfully created invoice {InvoiceId} for enrollment {EnrollmentId}",
                            result.Value.Id, enrollment.Id);
                        createdCount++;
                    }
                    else
                    {
                        _logger.LogError("Failed to create invoice for enrollment {EnrollmentId}. Errors: {Errors}",
                            enrollment.Id, string.Join(", ", result.Errors));
                        errorCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating invoice for enrollment {EnrollmentId}", enrollment.Id);
                    errorCount++;
                }
            }

            _logger.LogInformation(
                "Monthly invoice generation completed. Created: {Created}, Skipped: {Skipped}, Errors: {Errors}",
                createdCount, skippedCount, errorCount);
        }

        public async Task<Result> MarkInvoicesAsPaidAsync(
            IReadOnlyCollection<int> invoiceIds,
            int paymentId,
            CancellationToken cancellationToken)
        {
            var invoices = await _db.Invoices
                .Where(i => invoiceIds.Contains(i.Id))
                .ToListAsync(cancellationToken);

            if (!invoices.Any())
                return Result.NotFound("No invoices found");

            foreach (var invoice in invoices)
            {
                invoice.Status = InvoiceStatus.Paid;
                invoice.PaymentId = paymentId;
            }

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Marked {Count} invoices as paid with PaymentId {PaymentId}",
                invoices.Count, paymentId);

            return Result.Success();
        }

        private InvoiceStatus GetCurrentStatus(Invoice invoice)
        {
            if (invoice.PaymentId.HasValue)
                return InvoiceStatus.Paid;

            if (invoice.CancelledAt.HasValue)
            {
                return invoice.Status == InvoiceStatus.Withdrawn
                    ? InvoiceStatus.Withdrawn
                    : InvoiceStatus.Cancelled;
            }

            if (invoice.DueAt.HasValue && DateTime.UtcNow > invoice.DueAt.Value)
                return InvoiceStatus.Overdue;

            return InvoiceStatus.Pending;
        }

        private (bool IsValid, string ErrorMessage) ValidateStatusTransition(
            InvoiceStatus currentStatus,
            InvoiceStatus newStatus)
        {
            if (currentStatus == InvoiceStatus.Paid && newStatus != InvoiceStatus.Paid)
                return (false, "Cannot modify a paid invoice");

            if ((currentStatus == InvoiceStatus.Cancelled || currentStatus == InvoiceStatus.Withdrawn) &&
                newStatus != currentStatus)
                return (false, $"Cannot modify a {currentStatus.ToString().ToLower()} invoice");

            if (newStatus == InvoiceStatus.Overdue)
                return (false, "Overdue invoices are created automatically when due date passes");

            return (true, string.Empty);
        }
    }
}
