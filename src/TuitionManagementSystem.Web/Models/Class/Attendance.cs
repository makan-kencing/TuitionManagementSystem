namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using User;

[Index(nameof(SessionId), nameof(StudentId), IsUnique = true)]
public class Attendance
{
    [Key]
    public int Id { get; set; }

    public int SessionId { get; set; }

    public int StudentId { get; set; }

    public DateTime TakenOn { get; set; } = DateTime.UtcNow;

    public Session Session { get; set; } = null!;

    public Student Student { get; set; } = null!;
}
