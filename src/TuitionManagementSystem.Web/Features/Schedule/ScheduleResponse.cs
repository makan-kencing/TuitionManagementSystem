namespace TuitionManagementSystem.Web.Features.Schedule;

using Ical.Net;

public record ScheduleResponse(
    int Id,
    int CourseId,
    string? Summary,
    string? Description,
    DateTime Start,
    DateTime End);

public record ScheduleDetailResponse(
    int Id,
    int CourseId,
    string? Summary,
    string? Description,
    DateTime Start,
    DateTime End,
    IEnumerable<RecurrencePatternDto> RecurrencePatterns,
    IEnumerable<DateTime> RecurrenceDates,
    IEnumerable<DateTime> ExceptionDates);

public record ScheduleOccurrence(
    int ScheduleId,
    int CourseId,
    string CourseName,
    int ClassroomId,
    string ClassroomName,
    DateTime Start,
    DateTime End);
