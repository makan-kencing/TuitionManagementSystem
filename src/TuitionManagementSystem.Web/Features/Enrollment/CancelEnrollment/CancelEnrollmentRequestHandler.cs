namespace TuitionManagementSystem.Web.Features.Enrollment.CancelEnrollment;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CancelEnrollmentRequestHandler : IRequestHandler<CancelEnrollmentRequest, Result>
{
    private readonly ApplicationDbContext _db;

    public CancelEnrollmentRequestHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result> Handle(CancelEnrollmentRequest request, CancellationToken cancellationToken)
    {
        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId, cancellationToken);

        if (enrollment == null)
        {
            return Result.NotFound("Enrollment not found");
        }

        _db.Enrollments.Remove(enrollment);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
