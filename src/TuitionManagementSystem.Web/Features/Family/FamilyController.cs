using Microsoft.AspNetCore.Mvc;

namespace TuitionManagementSystem.Web.Features.Family;

using AcceptInvite;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CheckInvite;
using DeclineInvite;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ViewChild;
using ViewFamily;

[Authorize]
public class FamilyController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var invite = await mediator.Send(new CheckInviteQuery());
        if (invite.IsSuccess)
        {
            return this.View("Accept", new CheckInviteViewModel { Invite = invite.Value });
        }

        var family = await mediator.Send(new ViewFamilyQuery());
        return this.View(new ViewFamilyViewModel
        {
            Family = family.IsSuccess ? family.Value : null
        });
    }

    [HttpGet]
    public async Task<IActionResult> Child(int id)
    {
        var result = await mediator.Send(new ViewChildQuery(id));
        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> Accept()
    {
        var result = await mediator.Send(new AcceptInviteRequest());

        return this.RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Decline()
    {
        var result = await mediator.Send(new DeclineInviteRequest());

        return this.RedirectToAction("Index");
    }

}
