namespace TuitionManagementSystem.Web.Models.Payment;

public class GenericPaymentMethod : PaymentMethod
{
    public required string Name { get; set; }

    public override string GetSummaryText() => this.Name;
}
