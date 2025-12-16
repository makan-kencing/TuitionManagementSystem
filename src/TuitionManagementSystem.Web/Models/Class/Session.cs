namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class Session
{
    [Key]
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int ClassroomId { get; set; }

    public required DateTime StartAt { get; set; }

    public required DateTime EndAt { get; set; }

    public int? AttendanceCodeId { get; set; }

    public DateTime? CodeGeneratedAt { get; set; }

    public Classroom Classroom { get; set; } = null!;

    public Course Course { get; set; } = null!;

    public AttendanceCode? AttendanceCode { get; set; }

    public ICollection<Attendance> Attendances { get; set; } = [];
}
