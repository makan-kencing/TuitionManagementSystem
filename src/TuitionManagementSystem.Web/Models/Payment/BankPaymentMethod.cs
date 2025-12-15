namespace TuitionManagementSystem.Web.Models.Payment;

using System.ComponentModel.DataAnnotations;

public class BankPaymentMethod : PaymentMethod
{
    [StringLength(30)]
    public required string Bank { get; set; }

    public override string GetSummaryText() => this.Bank;
}
