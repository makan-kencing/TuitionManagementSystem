using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using MediatR;
using TuitionManagementSystem.Web.Models.ViewModels;
using TuitionManagementSystem.Web.Models.Payment;
using TuitionManagementSystem.Web.Features.Invoice.CreateInvoice;
using TuitionManagementSystem.Web.Features.Invoice.ListInvoice;

namespace TuitionManagementSystem.Web.Features.Invoice;

using Services.Auth.Extensions;

[ApiController]
[Route("invoice")]
public class InvoiceController : Controller
{
    private readonly IMediator _mediator;

    public InvoiceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListInvoices(
        [FromQuery] string? status = null,
        [FromQuery] bool? overdueOnly = null,
        [FromQuery] bool? pendingOnly = null,
        [FromQuery] int? month = null,
        [FromQuery] int? year = null,
        CancellationToken cancellationToken = default)
    {
        var studentId = User.GetUserId();

        InvoiceStatus? parsedStatus = null;

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<InvoiceStatus>(status, true, out var s))
            {
                return BadRequest(new
                {
                    errors = new[] { "Invalid status value. Allowed: Pending, Paid, Cancelled, Withdrawn." }
                });
            }

            parsedStatus = s;
        }

        var result = await _mediator.Send(
            new ListInvoiceRequest
            {
                StudentId = studentId,
                Status = parsedStatus,
                OverdueOnly = overdueOnly,
                PendingOnly = pendingOnly,
                Month = month,
                Year = year ?? DateTime.Now.Year
            },
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { errors = result.Errors });

        var availableMonths = result.Value
            .Where(i => i.InvoicedAt != default)
            .Select(i => i.InvoicedAt.Month)
            .Distinct()
            .OrderBy(m => m)
            .ToList();

        var invoices = result.Value
            .Select(invoice => new InvoiceViewModel
            {
                InvoiceId = invoice.Id,
                StudentId = invoice.Student?.Id ?? 0,
                StudentName = invoice.Student?.Account?.Username ?? "N/A",
                EnrollmentId = invoice.Enrollment?.Id ?? 0,
                SubjectName = invoice.Enrollment?.Course?.Subject?.Name ?? "N/A",
                CourseName = invoice.Enrollment?.Course?.Name ?? "N/A",
                Amount = invoice.Amount,
                InvoicedAt = invoice.InvoicedAt,
                DueAt = invoice.DueAt,
                Status = invoice.Status,
                IsOverdue =
                    invoice.Status == InvoiceStatus.Pending &&
                    invoice.DueAt.HasValue &&
                    invoice.DueAt.Value < DateTime.UtcNow
            })
            .OrderBy(i => i.InvoicedAt)
            .ThenBy(i => i.IsOverdue ? 0 : 1)
            .ToList();

        return View("ListInvoices",
            new ListInvoiceViewModel
            {
                Invoices = invoices,
                AvailableMonths = availableMonths,
                SelectedMonth = month,
                SelectedYear = year ?? DateTime.Now.Year,
                OverdueOnly = overdueOnly ?? false,
                PendingOnly = pendingOnly ?? false,
                SelectedStatus = status
            });
    }
}
