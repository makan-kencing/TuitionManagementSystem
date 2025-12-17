namespace TuitionManagementSystem.Web.Features.Family.CheckInvite;

public class CheckInviteResponse
{
    public required Family Family { get; init; }

    public required InviteParent Requester { get; init; }

    public required DateTime RequestedAt { get; init; }
}

public class Family
{
    public required string Name { get; set; }
}

public class InviteParent
{
    public required string AccountUsername { get; init; }

    public string? AccountDisplayName { get; init; }
}
