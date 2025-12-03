namespace TuitionManagementSystem.Web.Models.Payment;

using System.ComponentModel.DataAnnotations;

public abstract class PaymentMethod
{
    [Key]
    public int Id { get; set; }

    public abstract string GetSummaryText();

    public virtual Payment? Payment { get; set; }
}
