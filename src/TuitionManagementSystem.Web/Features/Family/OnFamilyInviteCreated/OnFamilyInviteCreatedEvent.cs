namespace TuitionManagementSystem.Web.Features.Family.OnFamilyInviteCreated;

using MediatR;
using TuitionManagementSystem.Web.Models.Notification;

public record OnFamilyInviteCreatedEvent(FamilyInvite Invite) : INotification;
