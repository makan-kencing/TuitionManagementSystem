using Microsoft.AspNetCore.Mvc;

namespace TuitionManagementSystem.Web.Features.Family;

using AcceptInvite;
using Ardalis.Result;
using CheckInvite;
using DeclineInvite;
using MediatR;
using ViewChild;
using ViewFamily;

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
        var result = await mediator.Send(new AcceptInviteCommand());

        return this.RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Decline()
    {
        var result = await mediator.Send(new DeclineInviteCommand());

        return this.RedirectToAction("Index");
    }

}
