namespace TuitionManagementSystem.Web.Features.Schedule;

using Ical.Net;
using Ical.Net.Serialization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("schedules")]
public class ScheduleIcsController(IMediator mediator) : Controller
{
    // Overall calendar feed
    [HttpGet("feed.ics")]
    public async Task<IActionResult> FeedAll(CancellationToken ct)
    {
        var schedules = await mediator.Send(new GetAllSchedulesForFeed(), ct);
        return IcsResult("timetable-all.ics", schedules);
    }

    // Course feed (for enrollment/details)
    [HttpGet("feed/course/{courseId:int}.ics")]
    public async Task<IActionResult> FeedCourse(int courseId, CancellationToken ct)
    {
        var schedules = await mediator.Send(new GetSchedulesForCourseFeed(courseId), ct);
        return IcsResult($"timetable-course-{courseId}.ics", schedules);
    }

    // Teacher feed
    [HttpGet("feed/teacher/{teacherId:int}.ics")]
    public async Task<IActionResult> FeedTeacher(int teacherId, CancellationToken ct)
    {
        var schedules = await mediator.Send(new GetSchedulesForTeacherFeed(teacherId), ct);
        return IcsResult($"timetable-teacher-{teacherId}.ics", schedules);
    }

    // Student feed
    [HttpGet("feed/student/{studentId:int}.ics")]
    public async Task<IActionResult> FeedStudent(int studentId, CancellationToken ct)
    {
        var schedules = await mediator.Send(new GetSchedulesForStudentFeed(studentId), ct);
        return IcsResult($"timetable-student-{studentId}.ics", schedules);
    }

    private FileContentResult IcsResult(string fileName, IReadOnlyList<ScheduleFeedItem> schedules)
    {
        var cal = new Calendar
        {
            ProductId = "-//TuitionManagementSystem//Schedules//EN",
            Method = "PUBLISH"
        };

        foreach (var s in schedules)
        {
            // This uses your existing RRULE/RDATE/EXDATE mapping
            var ev = s.Schedule.ToICalendarEvent();

            // Optional: stable UID helps clients dedupe
            ev.Uid = $"schedule-{s.Schedule.Id}@tms";

            // Optional: location if you want it visible in clients
            if (!string.IsNullOrWhiteSpace(s.ClassroomLocation))
                ev.Location = s.ClassroomLocation;

            // Optional: title fallback
            if (string.IsNullOrWhiteSpace(ev.Summary))
                ev.Summary = s.CourseName;

            cal.Events.Add(ev);
        }

        var serializer = new CalendarSerializer();
        var ics = serializer.SerializeToString(cal);

        return File(System.Text.Encoding.UTF8.GetBytes(ics), "text/calendar; charset=utf-8", fileName);
    }
}

// What the feed builder needs (you already have Schedule + Course + PreferredClassroom)
public sealed record ScheduleFeedItem(
    TuitionManagementSystem.Web.Models.Class.Schedule Schedule,
    string CourseName,
    string? ClassroomLocation
);
