namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;

public abstract class User
{
    [Key]
    public int Id { get; set; }
}
