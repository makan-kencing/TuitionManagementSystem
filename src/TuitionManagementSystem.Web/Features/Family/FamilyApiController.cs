namespace TuitionManagementSystem.Web.Features.Family;

using Abstractions;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemoveMember;
using SendFamilyInvite;
using Services.Auth.Extensions;

[Authorize]
public class FamilyApiController(IMediator mediator) : ApiController
{
    [HttpPost]
    [Route("invite")]
    public async Task<Result> Invite([FromForm] string username) =>
        await mediator.Send(new SendFamilyInviteCommand(this.User.GetUserId(), username));

    [HttpDelete]
    [Route("member/{id:required:int}")]
    public async Task<Result> RemoveMember([FromRoute] int id) =>
        await mediator.Send(new RemoveMemberCommand(this.User.GetUserId(), id));
}
