namespace TuitionManagementSystem.Web.Features.Accounts;

using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.User;
using Services.Auth.Extensions;

public class AccountController(ApplicationDbContext db, IMediator _mediator) : Controller
{
    public async Task<IActionResult> AccountProfile(CancellationToken cancellationToken)
    {
        var profile = await db.Accounts
            .Where(a => a.Id == this.User.GetUserId())
            .Select(a => new AccountProfileViewModel { Username = a.Username, Email = a.Email })
            .FirstOrDefaultAsync(cancellationToken);

        if (profile == null)
        {
            return this.Challenge();
        }

        return this.View(profile);
    }
}
