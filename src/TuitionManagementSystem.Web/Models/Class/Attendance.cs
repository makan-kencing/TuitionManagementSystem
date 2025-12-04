namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User;

public class Attendance
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Session) + "Id")]
    public required Session Session { get; set; }

    [ForeignKey(nameof(Student) + "Id")]
    public required Student Student { get; set; }

    public required DateTime TakenOn { get; set; } = DateTime.UtcNow;
}
