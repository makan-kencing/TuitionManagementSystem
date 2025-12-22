namespace TuitionManagementSystem.Web.Features.Child;

using Abstractions;
using Ardalis.Result;
using GetChild;
using GetChildrenEnrollment;
using Infrastructure.Persistence;
using Invoice.ListInvoice;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Payment;
using Models.User;
using Models.ViewModels;
using Services.Auth.Extensions;

public class ChildController(IMediator mediator, ApplicationDbContext db) : WebController
{
    [HttpGet]
    public async Task<IActionResult> Index(int id)
    {
        var result = await mediator.Send(new GetChildQuery(this.User.GetUserId(), id));

        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        if (result.IsForbidden())
        {
            return this.Forbid();
        }

        return this.View(new ChildViewModel { Child = result.Value });
    }

    public class ChildViewModel
    {
        public required GetChildResponse Child { get; set; }
    }

    [HttpGet]
    public async Task<IActionResult> Enrollment()
    {
        var response = await mediator.Send(new GetChildrenEnrollmentQuery(this.User.GetUserId()));
        if (response.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View(response.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Invoice(
        [FromQuery] string? status = null,
        [FromQuery] bool? overdueOnly = null,
        [FromQuery] bool? pendingOnly = null,
        [FromQuery] int? month = null,
        [FromQuery] int? year = null,
        CancellationToken cancellationToken = default)
    {
        var family = await db.Parents
            .AsNoTracking()
            .Where(p => p.Id == this.User.GetUserId())
            .Select(p => p.Family.Family)
            .FirstOrDefaultAsync(cancellationToken);

        if (family is null)
        {
            return this.NotFound();
        }

        var studentIds = await db.Students
            .Where(s => s.Family.Family.Id == family.Id)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        InvoiceStatus? parsedStatus = null;

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<InvoiceStatus>(status, true, out var s))
            {
                return this.BadRequest(new
                {
                    errors = new[] { "Invalid status value. Allowed: Pending, Paid, Cancelled, Withdrawn." }
                });
            }

            parsedStatus = s;
        }

        var resultValues = new List<Invoice>();

        foreach (var studentId in studentIds)
        {
            var result = await mediator.Send(
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
            {
                return this.BadRequest(new { errors = result.Errors });
            }

            resultValues.AddRange(result.Value);
        }

        var availableMonths = resultValues
            .Where(i => i.InvoicedAt != default)
            .Select(i => i.InvoicedAt.Month)
            .Distinct()
            .OrderBy(m => m)
            .ToList();

        var invoices = resultValues
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

        return this.View("ListInvoices",
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
