namespace TuitionManagementSystem.Web.Features.Authentication.Security;

public class VerifyOtpViewModel
{
    public string Otp { get; set; } = null!;
    public string? Message { get; set; }
}
