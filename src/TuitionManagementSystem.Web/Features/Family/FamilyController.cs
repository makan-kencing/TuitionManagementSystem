using Microsoft.AspNetCore.Mvc;

namespace TuitionManagementSystem.Web.Features.Family;

using AcceptInvite;
using Ardalis.Result;
using CheckInvite;
using DeclineInvite;
using GetChild;
using GetFamily;
using MediatR;

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

        var family = await mediator.Send(new GetFamilyQuery());
        if (family.IsNotFound())
        {
            return this.View("NoFamilyFound");
        }

        return this.View(new ViewFamilyViewModel
        {
            Family = family.Value
        });
    }

    [HttpGet]
    public async Task<IActionResult> Child(int id)
    {
        var result = await mediator.Send(new GetChildQuery(id));
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
