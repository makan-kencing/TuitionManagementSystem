namespace TuitionManagementSystem.Web.ViewModels.Schedule;

using System.ComponentModel.DataAnnotations;

public enum RepeatKind
{
    None = 0,
    Weekly = 1
}

public class ScheduleFormVm
{
    public int Id { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    [StringLength(50)]
    public string? Summary { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [Required]
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }

    public RepeatKind Repeat { get; set; } = RepeatKind.None;

    // Weekly options
    public int Interval { get; set; } = 1;
    public List<DayOfWeek> ByDay { get; set; } = [];

    public DateTime? Until { get; set; }

    // Manual lists (optional)
    public string? RecurrenceDatesText { get; set; }  // one date per line (yyyy-MM-dd)
    public string? ExceptionDatesText { get; set; }   // one date per line (yyyy-MM-dd)
}

public class ScheduleIndexVm
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int? CourseId { get; set; }
    public IEnumerable<TuitionManagementSystem.Web.Features.Schedule.ScheduleOccurrence> Occurrences { get; set; } = [];
}

public sealed class ScheduleCalendarVm
{
    public int? CourseId { get; set; }
    public int? TeacherId { get; set; }
    public int? StudentId { get; set; }
}
