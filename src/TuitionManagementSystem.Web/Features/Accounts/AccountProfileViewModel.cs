namespace TuitionManagementSystem.Web.Features.Accounts;

using Microsoft.AspNetCore.Mvc.Razor;

public class AccountProfileViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
