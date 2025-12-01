namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;

public class Classroom
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public required string Location { get; set; }

    public ICollection<Session> SessionsHeld { get; set; } = new List<Session>();

    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
