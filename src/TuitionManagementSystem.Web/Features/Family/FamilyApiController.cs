namespace TuitionManagementSystem.Web.Features.Family;

using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendFamilyInvite;

[Authorize]
[ApiController]
[Route("/api/family")]
public class FamilyApiController(IMediator mediator) : Controller
{
    [HttpPost]
    [Route("invite")]
    [TranslateResultToActionResult]
    public async Task<Result<SendFamilyInviteResponse>> Invite([FromForm] SendFamilyInviteViewModel model) =>
        await mediator.Send(new SendFamilyInviteRequest(model));
}
