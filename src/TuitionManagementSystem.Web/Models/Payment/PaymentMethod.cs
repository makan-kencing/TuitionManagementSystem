namespace TuitionManagementSystem.Web.Models.Payment;

using System.ComponentModel.DataAnnotations;

public abstract class PaymentMethod
{
    [Key]
    public int Id { get; set; }

    public Payment? Payment { get; set; }

    public abstract string GetSummaryText();
}
