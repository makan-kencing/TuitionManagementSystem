namespace TuitionManagementSystem.Web.Features.Family;

using Abstractions;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendFamilyInvite;

[Authorize]
public class FamilyApiController(IMediator mediator) : ApiController
{
    [HttpPost]
    [Route("invite")]
    public async Task<Result<SendFamilyInviteResponse>> Invite([FromForm] SendFamilyInviteCommand model) =>
        await mediator.Send(model);
}
