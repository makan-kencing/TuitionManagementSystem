namespace TuitionManagementSystem.Web.Features.Family;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Models.User;
using Services.Auth.Extensions;

public class FamilyAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Family>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Family resource)
    {
        var accountId = context.User.GetUserId() ?? -1;
        if (accountId == -1)  // cant check for null since compiler keep it nullable
        {
            return Task.CompletedTask;
        }

        if (requirement == FamilyOperations.View)
        {
            if (IsParentOf(resource, accountId) || IsChildOf(resource, accountId))
            {
                context.Succeed(requirement);
            }
        }
        else if (requirement == FamilyOperations.Invite
                 && requirement == FamilyOperations.EditDetails
                 && requirement == FamilyOperations.RemoveMember)
        {
            if (IsParentOf(resource, accountId))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }

    private static bool IsChildOf(Family family, int accountId) =>
        family.Children.Any(u => u.Account.Id == accountId);

    private static bool IsParentOf(Family family, int accountId) =>
        family.Parents.Any(u => u.Account.Id == accountId);
}

public static class FamilyOperations
{
    public static readonly OperationAuthorizationRequirement View = new(){ Name = nameof(View) };
    public static readonly OperationAuthorizationRequirement ViewChild = new() { Name = nameof(ViewChild) };
    public static readonly OperationAuthorizationRequirement Invite = new() { Name = nameof(Invite) };
    public static readonly OperationAuthorizationRequirement RemoveMember = new() { Name = nameof(RemoveMember) };
    public static readonly OperationAuthorizationRequirement EditDetails = new() { Name = nameof(EditDetails) };

}
