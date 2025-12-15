namespace TuitionManagementSystem.Web.Features.Invoice.CreateInvoice;

using Models.Payment;

public class CreateInvoiceResponse
{
    public int InvoiceId { get; set; }
    public int StudentId { get; set; }
    public int EnrollmentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime InvoicedAt { get; set; }
    public DateTime DueAt { get; set; }
    public InvoiceStatus Status { get; set; }
}
