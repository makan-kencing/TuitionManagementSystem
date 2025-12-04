namespace TuitionManagementSystem.Web.Models.Payment;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public required string Name { get; set; }

    public required decimal Amount { get; set; }

    [ForeignKey(nameof(Student) + "Id")]
    public required Student Student { get; set; }

    [ForeignKey(nameof(Payment) + "Id")]
    public Payment? Payment { get; set; }

    public required DateTime InvoicedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DueAt { get; set; }

    public bool IsPaid() => this.Payment != null;
}
