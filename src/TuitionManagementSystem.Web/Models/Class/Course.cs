namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
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

    [Precision(10, 2)]
    public decimal Price { get; set; }

    public int SubjectId { get; set; }

    public int PreferredClassroomId { get; set; }

    public Subject Subject { get; set; } = null!;

    public Classroom PreferredClassroom { get; set; } = null!;

    public Schedule? Schedule { get; set; }

    public ICollection<CourseTeacher> TeachersInCharge { get; set; } = [];

    public ICollection<Session> Sessions { get; set; } = [];

    public ICollection<Enrollment> Enrollments { get; set; } = [];

    public ICollection<Announcement.Announcement> Announcements { get; set; } = [];
}
