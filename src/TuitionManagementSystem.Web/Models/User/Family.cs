namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;

public class Family
{
    [Key]
    public int Id { get; set; }

    public ICollection<Parent> Parents { get; set; } = new List<Parent>();

    public ICollection<Student> Children { get; set; } = new List<Student>();
}
