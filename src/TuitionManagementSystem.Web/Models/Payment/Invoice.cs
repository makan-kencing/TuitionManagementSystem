namespace TuitionManagementSystem.Web.Models.Payment;

using System.ComponentModel.DataAnnotations;
using User;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public required string Name { get; set; }

    public required decimal Amount { get; set; }

    public int StudentId { get; set; }

    public int? PaymentId { get; set; }

    public DateTime InvoicedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DueAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public Student Student { get; set; } = null!;

    public Payment? Payment { get; set; }

    public InvoiceStatus InvoiceStatus
    {
        get
        {
            if (this.Payment is not null)
            {
                return InvoiceStatus.Paid;
            }

            if (DateTime.UtcNow > this.DueAt)
            {
                return InvoiceStatus.Overdue;
            }

            if (this.CancelledAt is not null)
            {
                return InvoiceStatus.Cancelled;
            }

            return InvoiceStatus.Pending;
        }
    }
}

public enum InvoiceStatus
{
    Pending,
    Paid,
    Cancelled,
    Overdue
}
