namespace TuitionManagementSystem.Web.Features.Family.SendFamilyInvite;

using System.ComponentModel.DataAnnotations;

public class SendFamilyInviteViewModel
{
    public string? Username { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}
