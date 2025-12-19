namespace TuitionManagementSystem.Web.Features.Notification;

using Abstractions;
using DismissNotification;
using GetNotifications;
using Htmx;
using MarkAllNotificationsDelivered;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Auth.Extensions;

public class NotificationController(IMediator mediator) : WebController
{
    [HttpGet]
    [Route("~/[controller]")]
    public async Task<IActionResult> GetNotifications()
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        var result = await mediator.Send(new GetNotificationsQuery(this.User.GetUserId()));
        await mediator.Send(new MarkAllNotificationsDeliveredCommand(this.User.GetUserId()));

        return this.PartialView("_NotificationDrawer", new NotificationsViewModel
        {
            Notifications = result.Value
        });
    }

    [HttpPost]
    [Route("~/[controller]/dismiss/{id:int:required}")]
    public async Task<IActionResult> DismissNotification(int id)
    {
        if (!this.Request.IsHtmx())
        {
            return this.NotFound();
        }

        await mediator.Send(new DismissNotificationCommand(this.User.GetUserId(), id));

        return await this.GetNotifications();
    }

    public class NotificationsViewModel
    {
        public required GetNotificationsResponse Notifications { get; set; }
    }
}
