namespace TuitionManagementSystem.Web.Features.Authentication.Registration;

using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

