namespace TuitionManagementSystem.Web.Features.Invoice.CreateInvoice;

using Ardalis.Result;
using MediatR;
using Models.Payment;

public class CreateInvoiceRequest : IRequest<Result<Invoice>>
{
    public int StudentId { get; set; }
    public int EnrollmentId { get; set; }
}
