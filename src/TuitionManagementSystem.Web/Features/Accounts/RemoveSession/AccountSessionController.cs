namespace TuitionManagementSystem.Web.Features.Accounts.RemoveSession;

using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[AllowAnonymous]
public class AccountSessionController(ApplicationDbContext db, IMediator mediator) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.Identity!.Name!);

        var sessions = await db.AccountSessions
            .Include(s => s.Account)
            .Where(s => s.AccountId == userId)
            .OrderByDescending(s => s.LastLogin)
            .ToListAsync();

        return View(sessions);
    }

    [HttpPost]
    public async Task<IActionResult> Remove(Guid sessionId)
    {
        var result = await mediator.Send(new RemoveSessionRequest(sessionId));

        if (!result.IsSuccess)
            return BadRequest(new { success = false, message = result.Errors.FirstOrDefault() });

        return Ok(new { success = true, message = result.Value!.Message });
    }
}
