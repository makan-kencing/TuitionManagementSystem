namespace TuitionManagementSystem.Web.UseCases.Mvc.Account.LoginAccount;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public sealed class LoginViewModel
{
    [Required]
    [EmailAddress]
    public required string Email { get; init;  }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; init;  }

    [DefaultValue(false)]
    public bool RememberMe { get; init; }
}
