namespace TuitionManagementSystem.Web.Models.Payment;

using System.ComponentModel.DataAnnotations;

public class CardPaymentMethod : PaymentMethod
{
    [StringLength(30)]
    public required string Brand { get; set; } // Visa, Amex, ...

    [StringLength(4)]
    public required string Last4 { get; set; }

    public override string GetSummaryText() => $"{this.Brand} ending in {this.Last4}";
}
