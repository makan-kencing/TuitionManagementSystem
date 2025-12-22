namespace TuitionManagementSystem.Web.Features.Invoice.ListInvoice;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Payment;

public class ListInvoiceRequestHandler : IRequestHandler<ListInvoiceRequest, Result<IEnumerable<Invoice>>>
{
    private readonly ApplicationDbContext db;

    public ListInvoiceRequestHandler(ApplicationDbContext db)
    {
        this.db = db;
    }

    public async Task<Result<IEnumerable<Invoice>>> Handle(ListInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var query = db.Invoices
            .Include(i => i.Student)
            .Include(i => i.Student.Account)
            .Include(i => i.Enrollment)
            .ThenInclude(e => e.Course)
            .ThenInclude(c => c.Subject)
            .AsQueryable();

            query = query.Where(i => i.Student.Id == request.StudentId);

        if (request.Status.HasValue)
            query = query.Where(i => i.Status == request.Status.Value);

        if (request.OverdueOnly.HasValue && request.OverdueOnly.Value)
        {
            var today = DateTime.UtcNow;
            query = query.Where(i =>
                i.Status == InvoiceStatus.Overdue &&
                i.DueAt.HasValue &&
                i.DueAt.Value < today);
        }

        if (request.PendingOnly.HasValue && request.PendingOnly.Value)
        {
            query = query.Where(i => i.Status == InvoiceStatus.Pending);
        }

        if (request.Month.HasValue)
        {
            query = query.Where(i => i.InvoicedAt.Month == request.Month.Value);
        }

        if (request.Year.HasValue)
        {
            query = query.Where(i => i.InvoicedAt.Year == request.Year.Value);
        }

        var invoices = await query.ToListAsync(cancellationToken);

        return Result.Success(invoices.AsEnumerable());
    }
}
