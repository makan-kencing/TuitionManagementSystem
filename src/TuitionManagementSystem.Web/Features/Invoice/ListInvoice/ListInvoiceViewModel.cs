namespace TuitionManagementSystem.Web.Features.Invoice.ListInvoice;

using Models.ViewModels;

public class ListInvoiceViewModel
{
    public List<InvoiceViewModel> Invoices { get; set; } = new();
    public List<int> AvailableMonths { get; set; } = new();
    public int? SelectedMonth { get; set; }
    public int SelectedYear { get; set; }
    public bool OverdueOnly { get; set; }
    public bool PendingOnly { get; set; }
    public string? SelectedStatus { get; set; }
    public decimal SelectedTotal => Invoices.Where(i => i.IsSelected).Sum(i => i.Amount);
    public int SelectedCount => Invoices.Count(i => i.IsSelected);
}
