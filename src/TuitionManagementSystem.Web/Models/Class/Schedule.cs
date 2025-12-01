namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Schedule
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public required DateOnly StartDate { get; set; }

    public required TimeOnly StartTime { get; set; }

    public required DateOnly EndDate { get; set; }

    public required TimeOnly EndTime { get; set; }

    public ScheduleRepeat? Repeat { get; set; }

    public DateTime? RepeatUntil { get; set; }

    public required Class Class { get; set; }
}

[Flags]
public enum ScheduleRepeat
{
    Daily,
    Weekly,
    EveryWeekday,
    BiWeekly,
    Monthly,
    Yearly
}
