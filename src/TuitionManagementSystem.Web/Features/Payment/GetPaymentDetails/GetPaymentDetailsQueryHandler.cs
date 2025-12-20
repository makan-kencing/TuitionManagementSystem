namespace TuitionManagementSystem.Web.Features.Payment.GetPaymentDetails;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Payment;

public class GetPaymentDetailsQueryHandler(ApplicationDbContext db)
    : IRequestHandler<GetPaymentDetailsQuery, Result<GetPaymentDetailsResponse>>
{
    public async Task<Result<GetPaymentDetailsResponse>> Handle(GetPaymentDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var paymentDetails = await db.Payments
            .Where(p => p.Id == request.PaymentId)
            .Select(p => new
            {
                p.Amount,
                p.PaidAt,
                p.Method,
                Invoices = p.Invoices.Select(i =>
                    new PaidInvoice
                    {
                        Name = i.Name,
                        Amount = i.Amount,
                        InvoicedAt = i.InvoicedAt,
                        Student = new InvoiceStudentDetails { Id = i.Student.Id, Name = i.Student.Account.Name },
                        Enrollment = new InvoiceEnrollmentDetails { CourseName = i.Enrollment.Course.Name }
                    }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (paymentDetails is null)
        {
            return Result.NotFound();
        }

        return Result.Success(new GetPaymentDetailsResponse
        {
            Amount = paymentDetails.Amount,
            PaidAt = paymentDetails.PaidAt,
            Method = paymentDetails.Method switch
            {
                CardPaymentMethod card => new CardMethod { Brand = card.Brand, Last4 = card.Last4 },
                BankPaymentMethod bank => new BankMethod { Bank = bank.Bank },
                GenericPaymentMethod generic => new GenericMethod { Generic = generic.Generic },
                _ => throw new InvalidCastException()
            },
            Invoices = paymentDetails.Invoices
        });
    }
}
