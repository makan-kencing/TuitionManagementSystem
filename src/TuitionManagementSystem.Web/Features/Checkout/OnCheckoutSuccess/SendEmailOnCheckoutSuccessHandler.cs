namespace TuitionManagementSystem.Web.Features.Checkout.OnCheckoutSuccess;

using System.Net.Mail;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Payment.GetPaymentDetails;
using Services.Email;
using Services.View;

public class SendEmailOnCheckoutSuccessHandler(
    ApplicationDbContext db,
    IRazorViewToStringRenderer view,
    IEmailService emailService)
    : INotificationHandler<OnCheckoutSuccessEvent>
{
    public async Task Handle(OnCheckoutSuccessEvent notification, CancellationToken cancellationToken)
    {

        var payment = await db.Payments
            .Where(p => p.Id == notification.PaymentId)
            .Select(p => new PaymentSuccessViewModel
            {
                Email = notification.Email,
                Name = notification.Name,
                ControllerContext = notification.ControllerContext,
                Invoices = p.Invoices
                    .Select(i => new PaidInvoice
                    {
                        Amount = i.Amount,
                        Enrollment = new InvoiceEnrollmentDetails { CourseName = i.Enrollment.Course.Name },
                        InvoicedAt = i.InvoicedAt,
                        Name = i.Name,
                        Student = new InvoiceStudentDetails { Id = i.Student.Id, Name = i.Student.Account.Name }
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (payment is null)
        {
            return;
        }

        var mail = new MailMessage
        {
            Subject = "Invoices Payment Receipt - Horizon Tuition Centre",
            To = { notification.Email },
            Body = await view.RenderViewToStringAsync("Checkout/E-Receipt", payment),
            IsBodyHtml = true
        };

        await emailService.SendAsync(mail, cancellationToken);
    }
}
