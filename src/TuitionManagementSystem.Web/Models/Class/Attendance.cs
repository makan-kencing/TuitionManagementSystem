namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using User;

public class Attendance
{
    [Key]
    public int Id { get; set; }

    public required Session Session { get; set; }

    public required Student Student { get; set; }

    public DateTime TakenOn { get; set; } = DateTime.Now;
}
