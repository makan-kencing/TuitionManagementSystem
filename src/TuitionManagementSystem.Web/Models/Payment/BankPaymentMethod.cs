namespace TuitionManagementSystem.Web.Models.Payment;

public class BankPaymentMethod : PaymentMethod
{
    public required string Bank { get; set; }

    public override string GetSummaryText() => this.Bank;
}
