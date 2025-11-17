namespace TuitionManagementSystem.Web.UseCases.Api.Account.CheckEmail;

using System.ComponentModel.DataAnnotations;

public sealed class CheckEmailResponse
{
    [Required]
    public bool Exists { get; init;  }
}
