namespace TuitionManagementSystem.Web.Features.Family.CheckInvite;

public class CheckInviteResponse
{
    public required FamilyInfo Family { get; init; }
    public required InviteParent Requester { get; init; }

    public required DateTime RequestedAt { get; init; }
}

public class FamilyInfo
{
    public required string Name { get; init; }
}

public class InviteParent
{
    public required ParentAccount Account { get; init; }
}

public class ParentAccount
{
    public required string Username { get; init; }
}
