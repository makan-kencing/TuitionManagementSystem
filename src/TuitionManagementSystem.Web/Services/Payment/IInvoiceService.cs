namespace TuitionManagementSystem.Web.Services
{
    using Ardalis.Result;
    using Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;
    using Models.Payment;

    public interface IInvoiceService
    {
        Task<Result<Invoice>> UpdateInvoiceStatusAsync(int invoiceId, InvoiceStatus newStatus, CancellationToken cancellationToken);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _db;

        public InvoiceService(ApplicationDbContext db)
        {
            _db = db;
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
                return Result.NotFound("Invoice not found");

            // Compute current status
            var currentStatus = ComputeCurrentStatus(invoice);

            // Validate status transition
            var validationResult = ValidateStatusTransition(currentStatus, newStatus);
            if (!validationResult.IsValid)
                return Result.Error(validationResult.ErrorMessage);

            // Update invoice
            switch (newStatus)
            {
                case InvoiceStatus.Cancelled:
                case InvoiceStatus.Withdrawn:
                    invoice.CancelledAt = DateTime.UtcNow;
                    invoice.Status = newStatus;
                    break;

                case InvoiceStatus.Paid:
                    // If you need to mark as paid later, you can implement this
                    return Result.Error("Use payment endpoint to mark invoice as paid");

                case InvoiceStatus.Overdue:
                    // Overdue should be automatic based on DueAt
                    return Result.Error("Overdue status is computed automatically");

                case InvoiceStatus.Pending:
                    // Reset cancellation if moving back to pending
                    invoice.CancelledAt = null;
                    invoice.Status = newStatus;
                    break;
            }

            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success(invoice);
        }

        private InvoiceStatus ComputeCurrentStatus(Invoice invoice)
        {
            // Simple computation without async loading
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
            // Can't modify paid invoices
            if (currentStatus == InvoiceStatus.Paid && newStatus != InvoiceStatus.Paid)
                return (false, "Cannot modify a paid invoice");

            // Can't modify cancelled/withdrawn invoices
            if ((currentStatus == InvoiceStatus.Cancelled || currentStatus == InvoiceStatus.Withdrawn) &&
                newStatus != currentStatus)
                return (false, $"Cannot modify a {currentStatus.ToString().ToLower()} invoice");

            // Can't manually set to overdue
            if (newStatus == InvoiceStatus.Overdue)
                return (false, "Overdue status is computed automatically");

            return (true, string.Empty);
        }
    }
}
