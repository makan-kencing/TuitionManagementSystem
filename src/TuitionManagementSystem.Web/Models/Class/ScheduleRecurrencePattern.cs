namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using Ical.Net;
using Ical.Net.DataTypes;

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
    public ICollection<int> BySecond { get; set; } = [];

    public ICollection<int> ByMinute { get; set; } = [];

    public ICollection<int> ByHour { get; set; } = [];

    public ICollection<DayOfWeek> ByDay { get; set; } = [];

    public ICollection<int> ByMonthDay { get; set; } = [];

    public ICollection<int> ByYearDay { get; set; } = [];

    public ICollection<int> ByWeekNo { get; set; } = [];

    public ICollection<int> ByMonth { get; set; } = [];

    public ICollection<int> BySetPosition { get; set; } = [];

    public int ScheduleId { get; set; }

    public Schedule Schedule { get; set; } = null!;

    public RecurrencePattern ToIRecurrencePattern() =>
        new()
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
