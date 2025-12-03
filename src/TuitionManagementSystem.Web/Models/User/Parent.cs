namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations.Schema;

public class Parent : User, IHasFamily
{
    [ForeignKey(nameof(Family) + "Id")]
    public Family? Family { get; set; }
}
