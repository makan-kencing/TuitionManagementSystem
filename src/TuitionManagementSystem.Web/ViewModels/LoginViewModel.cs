namespace TuitionManagementSystem.Web.ViewModels;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [EmailAddress]
    public required string Email { get; set; }

    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [DefaultValue(false)]
    public bool RememberMe { get; set; }
}
