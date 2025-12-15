namespace TuitionManagementSystem.Web.Models.Payment;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Class;
using User;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public required string Name { get; set; }

    public required decimal Amount { get; set; }

    public int StudentId { get; set; }

    public int EnrollmentId { get; set; }

    public int? PaymentId { get; set; }

    public DateTime InvoicedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DueAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

    [ForeignKey(nameof(StudentId))]
    public Student Student { get; set; } = null!;

    [ForeignKey(nameof(EnrollmentId))]
    public Enrollment Enrollment { get; set; } = null!;

    [ForeignKey(nameof(PaymentId))]
    public Payment? Payment { get; set; }
}

public enum InvoiceStatus
{
    Pending = 0,
    Paid = 1,
    Cancelled = 2,
    Overdue = 3,
    Withdrawn = 4
}
