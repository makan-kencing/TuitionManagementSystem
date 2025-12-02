namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using Announcement;
using User;

public class Course
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public required string Name { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public required Subject Subject { get; set; }

    public required Classroom PreferredClassroom { get; set; }

    public Schedule? Schedule { get; set; }

    public ICollection<Teacher> TeachersInCharge { get; set; } = new List<Teacher>();

    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public ICollection<Announcement.Announcement> Announcements { get; set; } = new List<Announcement.Announcement>();

    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
