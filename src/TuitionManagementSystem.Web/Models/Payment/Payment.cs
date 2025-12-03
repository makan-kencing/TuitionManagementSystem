namespace TuitionManagementSystem.Web.Models.Payment;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Method) + "Id")]
    public required PaymentMethod Method { get; set; }

    public required decimal Amount { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public required DateTime PaidAt { get; set; } = DateTime.Now;

    public virtual ICollection<Invoice> Invoices { get; set; } = [];
}
