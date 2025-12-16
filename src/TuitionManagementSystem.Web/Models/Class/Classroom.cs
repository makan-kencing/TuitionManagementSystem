namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class Classroom
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public required string Location { get; set; }

    public required int MaxCapacity { get; set; }

    public ICollection<Session> Sessions { get; set; } = [];

    public ICollection<Schedule> Schedules { get; set; } = [];
}
