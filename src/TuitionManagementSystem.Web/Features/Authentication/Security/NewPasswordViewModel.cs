namespace TuitionManagementSystem.Web.Features.Authentication.Security;

using System.ComponentModel.DataAnnotations;

public class NewPasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = "";
}
