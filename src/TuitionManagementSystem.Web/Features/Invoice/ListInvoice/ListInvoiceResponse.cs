namespace TuitionManagementSystem.Web.Features.Invoice.ListInvoice;

using Models.Payment;

public class ListInvoiceResponse
{
    public int InvoiceId { get; set; }
    public int StudentId { get; set; }
    public int EnrollmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime InvoicedAt { get; set; }
    public DateTime? DueAt { get; set; }
}
