namespace TuitionManagementSystem.Web.Services
{
    using Ardalis.Result;
    using Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;
    using Models.Payment;

    public interface IInvoiceService
    {
        public Task<Result<Invoice>> MarkInvoiceAsync(int invoiceId, InvoiceStatus status, CancellationToken cancellationToken);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _db;

        public InvoiceService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Result<Invoice>> MarkInvoiceAsync(int invoiceId, InvoiceStatus status, CancellationToken cancellationToken)
        {
            var invoice = await _db.Invoices
                .Include(i => i.Student)
                .Include(i => i.Enrollment)
                .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);

            if (invoice == null)
                return Result.NotFound("Invoice not found");

            if (invoice.Status == InvoiceStatus.Paid && status != InvoiceStatus.Paid)
                return Result.Error("Cannot modify a paid invoice");

            if (invoice.Status == InvoiceStatus.Cancelled && status != InvoiceStatus.Cancelled)
                return Result.Error("Cannot modify a cancelled invoice");

            invoice.Status = status;
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Success(invoice);
        }
    }
}
