namespace TuitionManagementSystem.Web.Features.Enrollment.MarkEnrollment;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using Models.Payment;
using TuitionManagementSystem.Web.Services;

public class MarkEnrollmentRequestHandler : IRequestHandler<MarkEnrollmentRequest, Result>
{
    private readonly ApplicationDbContext _db;
    private readonly IInvoiceService _invoiceService;

    public MarkEnrollmentRequestHandler(
        ApplicationDbContext db,
        IInvoiceService invoiceService)
    {
        _db = db;
        _invoiceService = invoiceService;
    }

    public async Task<Result> Handle(
        MarkEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Status != Enrollment.EnrollmentStatus.Cancelled &&
            request.Status != Enrollment.EnrollmentStatus.Withdrawn)
        {
            return Result.Error("Status must be either Cancelled or Withdrawn");
        }

        var enrollment = await _db.Enrollments
            .Include(e => e.Student)
            .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

        if (enrollment == null)
            return Result.NotFound("Enrollment not found");

        if (enrollment.Status == Enrollment.EnrollmentStatus.Cancelled)
            return Result.Conflict("Enrollment is already cancelled");

        if (enrollment.Status == Enrollment.EnrollmentStatus.Withdrawn)
            return Result.Conflict("Enrollment is already withdrawn");

        if (enrollment.Status == Enrollment.EnrollmentStatus.Completed)
            return Result.Error("Cannot modify a completed enrollment");

        bool isTeacher = request.CurrentUserType == nameof(Teacher);

        if (!isTeacher && request.Status == Enrollment.EnrollmentStatus.Withdrawn)
        {
            var hasBlockingInvoices = await _db.Invoices
                .AnyAsync(i =>
                    i.EnrollmentId == request.EnrollmentId &&
                    (i.Status == InvoiceStatus.Pending ||
                     i.Status == InvoiceStatus.Overdue) &&
                    i.CancelledAt == null,
                    cancellationToken);

            if (hasBlockingInvoices)
            {
                return Result.Error(
                    "Withdrawal is not allowed while there are pending or overdue invoices. " +
                    "Please settle outstanding invoices first.");
            }
        }

        var invoice = await _db.Invoices
            .Where(i => i.EnrollmentId == request.EnrollmentId)
            .OrderByDescending(i => i.InvoicedAt)
            .FirstOrDefaultAsync(cancellationToken);

        using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            enrollment.Status = request.Status;

            if (invoice != null)
            {
                var invoiceStatus = request.Status == Enrollment.EnrollmentStatus.Cancelled
                    ? InvoiceStatus.Cancelled
                    : InvoiceStatus.Withdrawn;

                var invoiceResult = await _invoiceService.UpdateInvoiceStatusAsync(
                    invoice.Id,
                    invoiceStatus,
                    cancellationToken);

                if (!invoiceResult.IsSuccess)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result.Error(
                        $"Failed to update invoice: {string.Join(", ", invoiceResult.Errors)}");
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Error($"Failed to update enrollment: {ex.Message}");
        }
    }
}
