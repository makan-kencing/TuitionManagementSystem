namespace TuitionManagementSystem.Web.Features.Authentication.Security;

using System.ComponentModel.DataAnnotations;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    public string? Otp { get; set; }

    public bool ShowOtpForm { get; set; } = false;

}
