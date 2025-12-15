namespace TuitionManagementSystem.Web.Features.Enrollment.MarkEnrollment
{
    using Ardalis.Result;
    using Infrastructure.Persistence;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models.Class;
    using Models.Payment;

    public class MarkEnrollmentRequestHandler : IRequestHandler<MarkEnrollmentRequest, Result>
    {
        private readonly ApplicationDbContext _db;

        public MarkEnrollmentRequestHandler(ApplicationDbContext db)
        {
            _db = db;
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

            var invoice = await _db.Invoices
                .FirstOrDefaultAsync(i =>
                    i.Enrollment.Id == request.EnrollmentId &&
                    i.Status != InvoiceStatus.Cancelled &&
                    i.Status != InvoiceStatus.Withdrawn,
                    cancellationToken);

            using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                enrollment.Status = request.Status;

                if (invoice != null)
                {
                    invoice.Status = request.Status switch
                    {
                        Enrollment.EnrollmentStatus.Cancelled => InvoiceStatus.Cancelled,
                        Enrollment.EnrollmentStatus.Withdrawn => InvoiceStatus.Withdrawn,
                        _ => invoice.Status
                    };
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
}
