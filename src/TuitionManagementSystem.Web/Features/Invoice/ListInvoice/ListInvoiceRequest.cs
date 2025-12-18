namespace TuitionManagementSystem.Web.Features.Invoice.ListInvoice;

using MediatR;
using Ardalis.Result;
using Models.Payment;

public class ListInvoiceRequest : IRequest<Result<IEnumerable<Invoice>>>
{
    public int StudentId { get; set; }
    public InvoiceStatus? Status { get; set; }
    public bool? OverdueOnly { get; set; }
    public bool? PendingOnly { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
}
