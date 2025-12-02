namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

// https://stackoverflow.com/questions/1054201/icalendar-field-list-for-database-schema-based-on-icalendar-standard/1397019#1397019
public class Schedule
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? Summary { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public required DateTime Start { get; set; }

    public required DateTime End { get; set; }

    public ICollection<ScheduleRecurrencePattern> RecurrencePatterns { get; set; } = new List<ScheduleRecurrencePattern>();

    public ICollection<DateTime> RecurrenceDates { get; set; } = new List<DateTime>();

    public ICollection<DateTime> ExceptionDates { get; set; } = new List<DateTime>();

    public CalendarEvent ToICalendarEvent()
    {
        var calendarEvent = new CalendarEvent
        {
            Summary = this.Summary,
            Description = this.Description,
            Start = this.Start.ToCalDateTime(),
            End = this.End.ToCalDateTime(),
            RecurrenceRules = this.RecurrencePatterns
                .Select(s => s.ToIRecurrencePattern())
                .ToList()
        };

        calendarEvent.RecurrenceDates.AddRange(this.RecurrenceDates
            .Select(v => v.ToCalDateTime()));
        calendarEvent.ExceptionDates.AddRange(this.ExceptionDates
            .Select(v => v.ToCalDateTime()));

        return calendarEvent;
    }
}

public class ScheduleRecurrencePattern
{
    [Key]
    public int Id { get; set; }

    public required FrequencyType FrequencyType { get; set; }

    // Sequence Based Rules
    public DateTime? Until { get; set; }

    public int? Count { get; set; }

    public int Interval { get; set; } = 1;

    // Calendar Based Rules
    public ICollection<int> BySecond { get; set; } = new List<int>();

    public ICollection<int> ByMinute { get; set; } = new List<int>();

    public ICollection<int> ByHour { get; set; } = new List<int>();

    public ICollection<DayOfWeek> ByDay { get; set; } = new List<DayOfWeek>();

    public ICollection<int> ByMonthDay { get; set; } = new List<int>();

    public ICollection<int> ByYearDay { get; set; } = new List<int>();

    public ICollection<int> ByWeekNo { get; set; } = new List<int>();

    public ICollection<int> ByMonth { get; set; } = new List<int>();

    public ICollection<int> BySetPosition { get; set; } = new List<int>();

    public RecurrencePattern ToIRecurrencePattern() =>
        new RecurrencePattern
        {
            Frequency = this.FrequencyType,
            Until = this.Until?.ToCalDateTime(),
            Count = this.Count,
            Interval = this.Interval,
            BySecond = this.BySecond.ToList(),
            ByMinute = this.ByMinute.ToList(),
            ByHour = this.ByHour.ToList(),
            ByDay = this.ByDay.Select(v => v.ToWeekDay()).ToList(),
            ByMonthDay = this.ByMonthDay.ToList(),
            ByYearDay = this.ByYearDay.ToList(),
            ByWeekNo = this.ByWeekNo.ToList(),
            ByMonth = this.ByMonth.ToList(),
            BySetPosition = this.BySetPosition.ToList()
        };
}

public static class ICalExtensions
{
    public static CalDateTime ToCalDateTime(this DateTime dateTime) => new(dateTime);
    public static WeekDay ToWeekDay(this DayOfWeek dayOfWeek) => new WeekDay(dayOfWeek);
}

