namespace TuitionManagementSystem.Web.Features.Accounts;

using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.User;
using Services.Auth.Extensions;

public class AccountController(ApplicationDbContext db) : Controller
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

               ProfileImageUrl = a.ProfileImage != null
                   ? a.ProfileImage.Uri
                   : "/assets/uploads/DefaultProfile.png"
           })
           .FirstOrDefaultAsync(cancellationToken);

       if (profile == null)
       {
           return this.Challenge();
       }

       return this.View(profile);
   }

}
