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
            // Get student
            var student = await db.Students
                .FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken);

            if (student == null)
                return Result.NotFound("Student not found");

            // Get enrollment with course details
            var enrollment = await db.Enrollments
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

            if (enrollment == null)
                return Result.NotFound("Enrollment not found");

            if (enrollment.Student.Id != request.StudentId)
                return Result.Error("Enrollment does not belong to the specified student");

            if (enrollment.Course == null)
                return Result.NotFound("Course not found");

            // Check if invoice already exists for this enrollment
            var existingInvoice = await db.Invoices
                .FirstOrDefaultAsync(i =>
                    i.EnrollmentId == request.EnrollmentId &&
                    i.CancelledAt == null, // Check if not cancelled
                    cancellationToken);

            if (existingInvoice != null)
                return Result.Conflict("An active invoice already exists for this enrollment");

            // Create invoice - DON'T set PaymentId or CancelledAt
            var invoice = new Models.Payment.Invoice
            {
                Name = $"INV-{enrollment.Course.Name}-{DateTime.UtcNow:yyyyMMdd}",
                Amount = enrollment.Course.Price,
                StudentId = student.Id,
                EnrollmentId = enrollment.Id,
                InvoicedAt = DateTime.UtcNow,
                DueAt = DateTime.UtcNow.AddDays(14),
                // PaymentId remains null (default)
                // CancelledAt remains null (default)
            };

            await db.Invoices.AddAsync(invoice, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            // Load related entities for the response
            await db.Entry(invoice)
                .Reference(i => i.Student)
                .LoadAsync(cancellationToken);

            await db.Entry(invoice)
                .Reference(i => i.Enrollment)
                .LoadAsync(cancellationToken);

            // Load Payment if exists (though it shouldn't for new invoice)
            await db.Entry(invoice)
                .Reference(i => i.Payment)
                .LoadAsync(cancellationToken);

            return Result.Success(invoice);
        }
    }
}
