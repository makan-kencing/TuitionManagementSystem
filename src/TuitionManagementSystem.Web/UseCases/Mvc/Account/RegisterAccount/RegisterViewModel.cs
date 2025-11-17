namespace TuitionManagementSystem.Web.UseCases.Mvc.Account.RegisterAccount;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class RegisterViewModel
{
    [Required]
    [StringLength(254)]
    [EmailAddress]
    [Remote("CheckEmail", "Account", ErrorMessage = "Diplicated {0}.")]
    public required string Email { get; init;  }

    [Required]
    [StringLength(500, MinimumLength = 12)]
    [DataType(DataType.Password)]
    public required string Password { get; init; }

    [Required]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public required string ConfirmPassword { get; init; }
}
