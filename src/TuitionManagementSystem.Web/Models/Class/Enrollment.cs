namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using User;

[Index(nameof(StudentId), nameof(CourseId), IsUnique = true)]
public class Enrollment
{
    [Key]
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    public Student Student { get; set; } = null!;

    public Course Course { get; set; } = null!;
}
