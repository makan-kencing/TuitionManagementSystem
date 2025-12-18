namespace TuitionManagementSystem.Web.Features.Schedule;

using Ical.Net;
using MediatR;

public record RecurrencePatternDto(
    FrequencyType FrequencyType,
    DateTime? Until,
    int? Count,
    int Interval,
    IEnumerable<DayOfWeek> ByDay
);

public record CreateSchedule(
    int CourseId,
    string? Summary,
    string? Description,
    DateTime Start,
    DateTime End,
    IEnumerable<RecurrencePatternDto> RecurrencePatterns,
    IEnumerable<DateTime> RecurrenceDates,
    IEnumerable<DateTime> ExceptionDates
) : IRequest<ScheduleResponse>;

public record UpdateSchedule(
    int Id,
    int CourseId,
    string? Summary,
    string? Description,
    DateTime Start,
    DateTime End,
    IEnumerable<RecurrencePatternDto> RecurrencePatterns,
    IEnumerable<DateTime> RecurrenceDates,
    IEnumerable<DateTime> ExceptionDates
) : IRequest<bool>;

public record DeleteSchedule(int Id) : IRequest<bool>;
public record GetSchedulesByCourse(int CourseId) : IRequest<IEnumerable<ScheduleResponse>>;
public record GetScheduleById(int Id) : IRequest<ScheduleDetailResponse?>;

public record GetScheduleOccurrencesByMonth(
    int? CourseId,
    int Year,
    int Month,
    int? TeacherId = null,
    int? StudentId = null)
    : IRequest<IEnumerable<ScheduleOccurrence>>;

public sealed record GetScheduleOccurrencesByDateRange(
    DateTime From,
    DateTime To,
    int? CourseId = null,
    int? TeacherId = null,
    int? StudentId = null
) : IRequest<IReadOnlyList<ScheduleOccurrence>>;

public record GetAllSchedulesForFeed() : IRequest<IReadOnlyList<ScheduleFeedItem>>;
public record GetSchedulesForCourseFeed(int CourseId) : IRequest<IReadOnlyList<ScheduleFeedItem>>;
public record GetSchedulesForTeacherFeed(int TeacherId) : IRequest<IReadOnlyList<ScheduleFeedItem>>;
public record GetSchedulesForStudentFeed(int StudentId) : IRequest<IReadOnlyList<ScheduleFeedItem>>;
