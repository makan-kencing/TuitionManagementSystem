namespace TuitionManagementSystem.Web.Features.Child;

using Abstractions;
using Ardalis.Result;
using GetChild;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Extensions;

public class ChildController(IMediator mediator) : WebController
{
    [HttpGet]
    public async Task<IActionResult> Index(int id)
    {
        var result = await mediator.Send(new GetChildQuery(this.User.GetUserId(), id));

        if (result.IsNotFound())
        {
            return this.NotFound();
        }

        if (result.IsForbidden())
        {
            return this.Forbid();
        }

        return this.View(new ChildViewModel
        {
            Child = result.Value
        });
    }

    public class ChildViewModel
    {
        public required GetChildResponse Child { get; set; }
    }
}
