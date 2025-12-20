namespace TuitionManagementSystem.Web.Features.User;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class UserProfileViewModel
{
    public int UserId { get; set; } // Needed to identify the user

    [Required]
    [DisplayName("Username")]
    [Remote("CheckUsernameUnique", "AccountsApi", AdditionalFields = "UserId", ErrorMessage = "Username is taken")]
    public required string Username { get; set; }

    [DisplayName("Display Name")]
    [StringLength(50)]
    public string? DisplayName { get; set; }

    [DisplayName("Email")]
    [EmailAddress]
    [Remote("CheckEmailUnique", "AccountsApi", AdditionalFields = "UserId", ErrorMessage = "Email is taken")]
    public string? Email { get; set; } = string.Empty;

    public string? ProfileImageUrl { get; set; }

    [DisplayName("Profile Image")]
    public IFormFile? ProfileImage { get; set; }
}
