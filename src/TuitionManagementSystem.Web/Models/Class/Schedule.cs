namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

// https://stackoverflow.com/questions/1054201/icalendar-field-list-for-database-schema-based-on-icalendar-standard/1397019#1397019
public class Schedule
{
    [Key] public int Id { get; set; }

    [StringLength(50)]
    public string? Summary { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public required DateTime Start { get; set; }

    public required DateTime End { get; set; }

    public int CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public ICollection<ScheduleRecurrencePattern> RecurrencePatterns { get; set; } = [];

    public ICollection<DateTime> RecurrenceDates { get; set; } = [];

    public ICollection<DateTime> ExceptionDates { get; set; } = [];

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

public static class ICalExtensions
{
    public static CalDateTime ToCalDateTime(this DateTime dateTime) => new(dateTime);
    public static WeekDay ToWeekDay(this DayOfWeek dayOfWeek) => new(dayOfWeek);
}
