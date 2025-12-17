namespace TuitionManagementSystem.Web.Features.Family;

using AcceptInvite;
using Ardalis.Result;
using CheckInvite;
using DeclineInvite;
using DeleteFamily;
using GetChild;
using GetFamily;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    [Route("~/[controller]/delete")]
    public IActionResult GetDelete()
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
}
