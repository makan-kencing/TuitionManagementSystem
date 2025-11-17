namespace TuitionManagementSystem.Web.UseCases.Api.Account.CheckEmail;

using Application.UseCases.CheckEmail;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]/[action]/{Id}")]
public sealed class AccountController(ILogger<AccountController> logger) : Controller, IOutputPort
{
    private IActionResult? viewModel;

    void IOutputPort.Ok(bool found) => this.viewModel = this.Ok(new CheckEmailResponse { Exists = found });

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CheckEmailResponse))]
    public async Task<IActionResult> CheckEmail(
        [FromServices] ICheckEmailUseCase useCase,
        string id)
    {
        useCase.SetOutputPort(this);

        await useCase.Execute(id);

        return this.viewModel ?? this.BadRequest();
    }
}
