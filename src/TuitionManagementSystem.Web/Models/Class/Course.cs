namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Name), IsUnique = true)]
public class Course
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public required string Name { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [ForeignKey(nameof(Subject) + "Id")]
    public required Subject Subject { get; set; }

    [ForeignKey(nameof(PreferredClassroom) + "Id")]
    public required Classroom PreferredClassroom { get; set; }

    public virtual Schedule? Schedule { get; set; }

    public virtual ICollection<CourseTeacher> TeachersInCharge { get; set; } = [];

    public virtual ICollection<Session> Sessions { get; set; } = [];

    public virtual ICollection<Enrollment> Enrollments { get; set; } = [];

    public virtual ICollection<Announcement.Announcement> Announcements { get; set; } = [];
}
