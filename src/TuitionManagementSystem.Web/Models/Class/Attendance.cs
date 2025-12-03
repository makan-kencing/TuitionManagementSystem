namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User;

public class Attendance
{
    [Key]
    public int Id { get; set; }

    public required int SessionId { get; set; }
    public required Session Session { get; set; }

    public required int StudentId { get; set; }
    public required Student Student { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public required DateTime TakenOn { get; set; } = DateTime.Now;
}
