namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;

public class Subject
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public required string Name { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = [];
}
