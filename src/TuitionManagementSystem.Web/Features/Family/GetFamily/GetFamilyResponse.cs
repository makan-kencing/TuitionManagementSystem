namespace TuitionManagementSystem.Web.Features.Family.GetFamily;

public class GetFamilyResponse
{
    public required string Name { get; set; }

    public ICollection<FamilyMember> Members { get; set; } = [];
}

public class FamilyMember
{
    public required FamilyUser User { get; set; }

    public required DateTime JoinedOn { get; set; }
}

public class FamilyUser
{
    public int Id { get; set; }

    public required string AccountUsername { get; set; }

    public string? AccountDisplayName { get; set; }

    public Uri? AccountProfileImageUri { get; set; }

    public required string Type { get; set; }
}
