namespace TuitionManagementSystem.Web.Features.Accounts;

using System.Net.Mime;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CheckEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/account")]
public sealed class AccountsApiController(ILogger<AccountsApiController> logger, IMediator mediator) : Controller
{
    [HttpGet("email/{email}")]
    [TranslateResultToActionResult]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CheckEmailResponse))]
    public async Task<Result<CheckEmailResponse>> CheckEmail([FromRoute] string email) =>
        await mediator.Send(new CheckEmailRequest(email));
}
