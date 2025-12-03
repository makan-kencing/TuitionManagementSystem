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

    public required int SubjectId { get; set; }
    public required Subject Subject { get; set; }

    public required int PreferredClassroomId { get; set; }
    public required Classroom PreferredClassroom { get; set; }

    public virtual Schedule? Schedule { get; set; }

    public virtual ICollection<CourseTeacher> TeachersInCharge { get; set; } = [];

    public virtual ICollection<Session> Sessions { get; set; } = [];

    public virtual ICollection<Enrollment> Enrollments { get; set; } = [];

    public virtual ICollection<Announcement.Announcement> Announcements { get; set; } = [];

    public virtual ICollection<Assignment> Assignments { get; set; } = [];

    public virtual ICollection<Material> Materials { get; set; } = [];
}

public class CourseTeacher
{
    [Key]
    public int Id { get; set; }

    public required int CourseId { get; set; }
    public required Course Course { get; set; }

    public required int TeacherId { get; set; }
    public required Teacher Teacher { get; set; }
}
