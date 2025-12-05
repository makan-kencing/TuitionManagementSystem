namespace TuitionManagementSystem.Web.Features.Authentication.Login;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public sealed class LoginViewModel
{
    [Required]
    [StringLength(30)]
    [DisplayName("Username")]
    public required string Username { get; init; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(300)]
    [DisplayName("Password")]
    public required string Password { get; init; }

    [StringLength(6)]
    [RegularExpression(@"^\d{6}$")]
    [DisplayName("Two Factor Token")]
    public string? TwoFactorToken { get; set; }

    [DisplayName("Remember Me")]
    public bool RememberMe { get; init; }
}
