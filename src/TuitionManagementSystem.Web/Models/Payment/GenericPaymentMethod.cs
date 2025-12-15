namespace TuitionManagementSystem.Web.Models.Payment;

using System.ComponentModel.DataAnnotations;

public class GenericPaymentMethod : PaymentMethod
{
    [StringLength(50)]
    public required string Generic { get; set; }

    public override string GetSummaryText() => this.Generic;
}
