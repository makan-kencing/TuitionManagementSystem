namespace TuitionManagementSystem.Web.Features.Accounts;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class AccountProfileViewModel
{
    [Required]
    [DisplayName("Username")]
    [Remote("CheckUsernameUnique", "AccountsApi", ErrorMessage = "Username is taken")]
    public required string Username { get; set; }

    [DisplayName("Email")]
    [EmailAddress]
    [Remote("CheckEmailUnique", "AccountsApi", ErrorMessage = "Email is taken")]
    public string? Email { get; set; } = string.Empty;

    [DisplayName("Password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required]
    [DisplayName("Confirm Password")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; } = string.Empty;
}

public class HasDigit : RegularExpressionAttribute
{
    public HasDigit() : base(@"\d")
    {
    }
}

public class HasAlphabet : RegularExpressionAttribute
{
    public HasAlphabet() : base("[a-zA-Z]")
    {
    }
}
