namespace TuitionManagementSystem.Web.Features.Family;

using System.ComponentModel;
using AcceptInvite;
using Ardalis.Result;
using CheckInvite;
using DeclineInvite;
using DeleteFamily;
using GetChild;
using GetFamily;
using Htmx;
using Infrastructure.Persistence;
using LeaveFamily;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RemoveMember;
using SendFamilyInvite;
using Services.Auth.Extensions;

public class FamilyController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var invite = await mediator.Send(new CheckInviteQuery(this.User.GetUserId()));
        if (invite.IsSuccess)
        {
            return this.View("Accept", new CheckInviteViewModel { Invite = invite.Value });
        }

        var family = await mediator.Send(new GetFamilyQuery(this.User.GetUserId()));
        if (family.IsNotFound())
        {
            return this.View("NoFamilyFound");
        }

        return this.View(new ViewFamilyViewModel { Family = family.Value });
    }

    [HttpGet]
    public async Task<IActionResult> Child(int id)
    {
        var result = await mediator.Send(new GetChildQuery(this.User.GetUserId(), id));
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> Accept()
    {
        await mediator.Send(new AcceptInviteCommand(this.User.GetUserId()));
        return this.RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Decline()
    {
        await mediator.Send(new DeclineInviteCommand(this.User.GetUserId()));
        return this.RedirectToAction("Index");
    }

    // Partials

    [HttpGet]
    [Route("~/[controller]/invite")]
    public IActionResult GetInvite()
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        return this.PartialView("Dialog/_Invite");
    }

    [HttpPost]
    [Route("~/[controller]/invite")]
    public async Task<IActionResult> Invite([FromForm] string username)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        var response = await mediator.Send(new SendFamilyInviteCommand(this.User.GetUserId(), username));
        if (response.IsConflict())
        {
            return this.PartialView("Toast/Invite/_PendingInviteToast");
        }

        if (response.IsForbidden())
        {
            return this.PartialView("Toast/Invite/_MemberAlreadyHasFamily");
        }

        return this.PartialView("Toast/Invite/_InviteSuccessToast");
    }

    public class InviteFamilyViewModel
    {
        [DisplayName("Username")]
        public required string Username { get; set; }
    }

    [HttpGet]
    [Route("~/[controller]/delete")]
    public IActionResult GetDeleteFamily()
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        return this.PartialView("Dialog/_DeleteFamily");
    }

    [HttpDelete]
    [Route("~/[controller]")]
    public async Task<IActionResult> DeleteFamily()
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        await mediator.Send(new DeleteFamilyCommand(this.User.GetUserId()));

        this.Response.Htmx(h => h.Redirect(this.Url.Action("Index")!));
        return this.Ok();
    }

    [HttpGet]
    [Route("~/[controller]/leave")]
    public IActionResult GetLeaveFamily()
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        return this.PartialView("Dialog/_LeaveFamily");
    }

    [HttpPost]
    [Route("~/[controller]/leave")]
    public async Task<IActionResult> LeaveFamily()
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        await mediator.Send(new LeaveFamilyCommand(this.User.GetUserId()));

        this.Response.Htmx(h => h.Redirect(this.Url.Action("Index")!));
        return this.Ok();
    }

    [HttpGet]
    [Route("~/[controller]/member/remove/{id:int:required}")]
    public async Task<IActionResult> GetRemoveMember([FromServices] ApplicationDbContext db, int id)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        return this.PartialView("Dialog/_RemoveMember", await db.Users
            .Where(u => u.Id == id)
            .Select(u => new RemoveMemberViewModel
            {
                Id = u.Id,
                Username = u.Account.Username,
                DisplayName = u.Account.DisplayName,
                Email = u.Account.Email,
                Type = u.GetType().Name
            })
            .FirstAsync());
    }

    public class RemoveMemberViewModel
    {
        public int Id { get; set; }

        public required string Username { get; set; }

        public string? DisplayName { get; set; }

        public string? ProfileImageUri { get; set; }

        public string? Email { get; set; }

        public string? Type { get; set; }
    }

    [HttpDelete]
    [Route("~/[controller]/member/{id:int:required}")]
    public async Task<IActionResult> RemoveMember(int id)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        await mediator.Send(new RemoveMemberCommand(this.User.GetUserId(), id));

        return this.PartialView("Toast/_MemberRemoveSuccessToast", new MemberRemoveSuccessViewModel
        {
            RemovedUserId = id
        });
    }

    public class MemberRemoveSuccessViewModel
    {
        public int RemovedUserId { get; set; }
    }
}
