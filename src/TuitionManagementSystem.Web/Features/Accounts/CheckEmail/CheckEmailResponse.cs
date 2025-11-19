namespace TuitionManagementSystem.Web.Features.Accounts.CheckEmail;

using System.ComponentModel.DataAnnotations;

public sealed class CheckEmailResponse
{
    [Required]
    public bool Exists { get; init;  }
}
