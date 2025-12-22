namespace TuitionManagementSystem.Web.Features.Accounts;

using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.User;
using RemoveSession;
using Services.Auth.Extensions;

public class AccountController(ApplicationDbContext db, IMediator mediator) : Controller
{
   public async Task<IActionResult> AccountProfile(CancellationToken cancellationToken)
   {
       var accountId = this.User.GetAccountId();

       var profile = await db.Accounts
           .Where(a => a.Id == accountId)
           .Select(a => new AccountProfileViewModel
           {
               Username = a.Username,
               Email = a.Email,
               DisplayName = a.DisplayName,

               IsTwoFactorEnabled = a.IsTwoFactorEnabled,

               ProfileImageUrl = a.ProfileImage != null
                   ? a.ProfileImage.Uri
                   : "/assets/img/DefaultProfile.png"

           })
           .FirstOrDefaultAsync(cancellationToken);

       if (profile == null)
       {
           return this.Challenge();
       }

       return this.View(profile);
   }

   [HttpGet("account/manage-sessions")]
   public async Task<IActionResult> ManageSession()
   {
       var userId = this.User.GetAccountId();
       var currentSessionId = Guid.Parse(HttpContext.User.Claims
           .First(c => c.Type == ClaimTypes.Thumbprint).Value);

       var sessions = await db.AccountSessions
           .Include(s => s.Account)
           .Where(s => s.AccountId == userId)
           .OrderByDescending(s => s.LastLogin)
           .ToListAsync();

       // pass current session ID to the view using ViewData
       ViewData["CurrentSessionId"] = currentSessionId;

       return View("~/Views/Account/ManageSession.cshtml", sessions);
   }

   [HttpPost("account/remove-session")]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> RemoveSession([FromForm] Guid sessionId)
   {
       var result = await mediator.Send(new RemoveSessionRequest(sessionId));

       if (!result.IsSuccess)
           return BadRequest(new { success = false, message = result.Errors.FirstOrDefault() });

       return Ok(new { success = true, message = result.Value!.Message });
   }


}
