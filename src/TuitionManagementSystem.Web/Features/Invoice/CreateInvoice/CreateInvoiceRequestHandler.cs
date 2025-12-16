namespace TuitionManagementSystem.Web.Features.Invoice.CreateInvoice
{
    using Ardalis.Result;
    using Infrastructure.Persistence;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models.Payment;
    using Models.Class;

    public class CreateInvoiceRequestHandler(ApplicationDbContext db)
        : IRequestHandler<CreateInvoiceRequest, Result<Models.Payment.Invoice>>
    {
        public async Task<Result<Models.Payment.Invoice>> Handle(
            CreateInvoiceRequest request,
            CancellationToken cancellationToken)
        {
            if (request.OverdueFromInvoiceId.HasValue)
            {
                return await CreateOverdueInvoiceAsync(
                    request.OverdueFromInvoiceId.Value,
                    cancellationToken);
            }

            return await CreateNewEnrollmentInvoiceAsync(
                request.StudentId,
                request.EnrollmentId,
                cancellationToken);
        }

        private async Task<Result<Invoice>> CreateOverdueInvoiceAsync(
            int overdueFromInvoiceId,
            CancellationToken cancellationToken)
        {
            var originalInvoice = await db.Invoices
                .Include(i => i.Student)
                .Include(i => i.Enrollment)
                .ThenInclude(e => e.Course)
                .Include(i => i.Payment)
                .FirstOrDefaultAsync(i => i.Id == overdueFromInvoiceId, cancellationToken);

            if (originalInvoice == null)
                return Result.NotFound("Original invoice not found");

            if (!originalInvoice.DueAt.HasValue || DateTime.UtcNow <= originalInvoice.DueAt.Value)
                return Result.Error("Original invoice is not yet overdue");

            if (originalInvoice.PaymentId.HasValue)
                return Result.Error("Cannot create overdue invoice from paid invoice");

            if (originalInvoice.CancelledAt.HasValue)
                return Result.Error("Cannot create overdue invoice from cancelled invoice");

            var existingOverdueInvoice = await db.Invoices
                .FirstOrDefaultAsync(i =>
                        i.EnrollmentId == originalInvoice.EnrollmentId &&
                        i.InvoicedAt.Date == originalInvoice.InvoicedAt.Date &&
                        i.DueAt == originalInvoice.DueAt &&
                        i.Status == InvoiceStatus.Overdue &&
                        i.CancelledAt == null,
                    cancellationToken);

            if (existingOverdueInvoice != null)
                return Result.Conflict("An overdue invoice already exists for this billing period");

            var interestRate = 0.03m; // 3% interest
            var interestAmount = originalInvoice.Amount * interestRate;
            var newAmount = originalInvoice.Amount + interestAmount;

            var overdueInvoice = new Invoice
            {
                Name = originalInvoice.Name.Replace("INV-", "INV-OVERDUE-"),
                Amount = newAmount,
                StudentId = originalInvoice.StudentId,
                EnrollmentId = originalInvoice.EnrollmentId,
                InvoicedAt = originalInvoice.InvoicedAt,
                DueAt = originalInvoice.DueAt,
                Status = InvoiceStatus.Overdue,
            };

            // mark original invoice as cancelled
            originalInvoice.CancelledAt = DateTime.UtcNow;
            originalInvoice.Status = InvoiceStatus.Cancelled;

            await db.Invoices.AddAsync(overdueInvoice, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            await db.Entry(overdueInvoice)
                .Reference(i => i.Student)
                .LoadAsync(cancellationToken);

            await db.Entry(overdueInvoice)
                .Reference(i => i.Enrollment)
                .LoadAsync(cancellationToken);

            return Result.Success(overdueInvoice);
        }

        private async Task<Result<Invoice>> CreateNewEnrollmentInvoiceAsync(
            int studentId,
            int enrollmentId,
            CancellationToken cancellationToken)
        {
            var student = await db.Students
                .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);

            if (student == null)
                return Result.NotFound("Student not found");

            var enrollment = await db.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId, cancellationToken);

            if (enrollment == null)
                return Result.NotFound("Enrollment not found");

            if (enrollment.Student.Id != studentId)
                return Result.Error("Enrollment does not belong to the specified student");

            if (enrollment.Course == null)
                return Result.NotFound("Course not found");

            var latestActiveInvoice = await db.Invoices
                .Where(i => i.EnrollmentId == enrollmentId &&
                            i.CancelledAt == null)
                .OrderByDescending(i => i.InvoicedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestActiveInvoice != null)
            {
                if (latestActiveInvoice.Status == InvoiceStatus.Pending)
                {
                    return Result.Conflict(
                        $"Cannot create new invoice while invoice #{latestActiveInvoice.Id} is pending");
                }
            }

            var invoice = new Invoice
            {
                Name = $"INV-{enrollment.Course.Name}-{DateTime.UtcNow:yyyyMMdd}",
                Amount = enrollment.Course.Price,
                StudentId = student.Id,
                EnrollmentId = enrollment.Id,
                InvoicedAt = DateTime.UtcNow,
                DueAt = DateTime.UtcNow.AddDays(14),
                Status = InvoiceStatus.Pending,
            };

            await db.Invoices.AddAsync(invoice, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            await db.Entry(invoice)
                .Reference(i => i.Student)
                .LoadAsync(cancellationToken);

            await db.Entry(invoice)
                .Reference(i => i.Enrollment)
                .LoadAsync(cancellationToken);

            return Result.Success(invoice);
        }
    }
}
