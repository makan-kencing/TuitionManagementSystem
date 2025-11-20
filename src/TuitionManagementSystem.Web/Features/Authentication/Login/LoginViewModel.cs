namespace TuitionManagementSystem.Web.Features.Authentication.Login;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public sealed class LoginViewModel
{
    [Required]
    [StringLength(30)]
    public required string Username { get; init;  }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(300)]
    public required string Password { get; init;  }

    [DefaultValue(false)]
    public bool RememberMe { get; init; }
}
