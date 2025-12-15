namespace TuitionManagementSystem.Web.Models.Payment;

using System.ComponentModel.DataAnnotations;

public class Payment
{
    [Key]
    public int Id { get; set; }

    public int MethodId { get; set; }

    public required decimal Amount { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    public PaymentMethod Method { get; set; } = null!;

    public ICollection<Invoice> Invoices { get; set; } = [];
}
