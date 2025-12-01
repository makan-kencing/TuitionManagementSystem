namespace TuitionManagementSystem.Web.Models.Payment;

public class CardPaymentMethod : PaymentMethod
{
    public required string Brand { get; set; }  // Visa, Amex, ...

    public required string Last4 { get; set; }

    public override string GetSummaryText() => $"{this.Brand} ending in {this.Last4}";
}
