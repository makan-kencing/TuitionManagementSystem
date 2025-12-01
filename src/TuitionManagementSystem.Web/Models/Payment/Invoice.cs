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

    public required Student Student { get; set; }

    public DateTime InvoicedAt { get; set; } = DateTime.Now;

    public required DateTime DueAt { get; set; }

    public Payment? Payment { get; set; }

    public bool IsPaid() => this.Payment != null;
}
