namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;

public class Family
{
    [Key]
    public int Id { get; set; }

    public virtual ICollection<Parent> Parents { get; set; } = [];

    public virtual ICollection<Student> Children { get; set; } = [];
}
