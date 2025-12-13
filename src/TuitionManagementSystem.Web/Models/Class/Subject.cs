namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Name), IsUnique = true)]

public class Subject : ISoftDeletable
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]

    public required string Name { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = [];
    public DateTime? DeletedAt { get; set; }
}
