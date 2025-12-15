namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using User;

[Index(nameof(CourseId), nameof(TeacherId), IsUnique = true)]
public class CourseTeacher
{
    [Key] public int Id { get; set; }

    public int CourseId { get; set; }

    public int TeacherId { get; set; }

    public Course Course { get; set; } = null!;

    public Teacher Teacher { get; set; } = null!;
}
