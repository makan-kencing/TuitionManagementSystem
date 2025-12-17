namespace TuitionManagementSystem.Web.Features.Family.OnFamilyInviteCreated;

using MediatR;
using Models.Notification;

public record OnFamilyInviteCreatedEvent(FamilyInvite Invite) : INotification;
