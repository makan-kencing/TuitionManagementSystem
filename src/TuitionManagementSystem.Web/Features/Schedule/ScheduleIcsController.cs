namespace TuitionManagementSystem.Web.Features.Schedule;

using System.Security.Claims;
using Ical.Net;
using Ical.Net.Serialization;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.User;
using Services.Auth.Constants;

[Authorize]
[Route("schedules")]
public class ScheduleIcsController(IMediator mediator, ApplicationDbContext db) : Controller
{


    // Overall calendar feed
    [HttpGet("feed.ics")]
    public async Task<IActionResult> FeedAll(CancellationToken ct)
    {
        var accountId = GetAccountId();
        if (!await IsAdminAsync(accountId, ct)) return Forbid();
        var schedules = await mediator.Send(new GetAllSchedulesForFeed(), ct);
        return IcsResult(
            fileName: "tms-timetable-all.ics",
            calendarName: "TMS — Timetable (All)",
            schedules);
    }

    // Course feed (for enrollment/details)
    [HttpGet("feed/course/{courseId:int}.ics")]
    public async Task<IActionResult> FeedCourse(int courseId, CancellationToken ct)
    {
        var schedules = await mediator.Send(new GetSchedulesForCourseFeed(courseId), ct);
        return IcsResult(
            fileName: $"tms-timetable-course-{courseId}.ics",
            calendarName: $"TMS — Course #{courseId} Timetable",
            schedules);
    }

    // Teacher feed
    [HttpGet("feed/teacher/{teacherId:int}.ics")]
    public async Task<IActionResult> FeedTeacher(int teacherId, CancellationToken ct)
    {
        var accountId = GetAccountId();
        var isAdmin = await IsAdminAsync(accountId, ct);

        if (!isAdmin)
        {
            var myTeacherId = await db.Users.OfType<Teacher>()
                .Where(t => t.AccountId == accountId)
                .Select(t => (int?)t.Id)
                .SingleOrDefaultAsync(ct);

            if (myTeacherId is null) return Forbid();

            teacherId = myTeacherId.Value;
        }

        var schedules = await mediator.Send(new GetSchedulesForTeacherFeed(teacherId), ct);

        return IcsResult(
            fileName: $"tms-timetable-teacher-{teacherId}.ics",
            calendarName: $"TMS — Teacher #{teacherId} Timetable",
            schedules);
    }

    // Student feed
    [HttpGet("feed/student/{studentId:int}.ics")]
    public async Task<IActionResult> FeedStudent(int studentId, CancellationToken ct)
    {
        var accountId = GetAccountId();
        var isAdmin = await IsAdminAsync(accountId, ct);

        if (!isAdmin)
        {
            var myStudentId = await db.Users.OfType<Student>()
                .Where(s => s.AccountId == accountId)
                .Select(s => (int?)s.Id)
                .SingleOrDefaultAsync(ct);

            if (myStudentId is null) return Forbid();

            studentId = myStudentId.Value;
        }

        var schedules = await mediator.Send(new GetSchedulesForStudentFeed(studentId), ct);

        return IcsResult(
            fileName: $"tms-timetable-student-{studentId}.ics",
            calendarName: $"TMS — Student #{studentId} Timetable",
            schedules);
    }

    private int GetAccountId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var accountId))
            throw new InvalidOperationException("Missing AccountId claim.");
        return accountId;
    }

    private Task<bool> IsAdminAsync(int accountId, CancellationToken ct) =>
        db.Accounts
            .Where(a => a.Id == accountId)
            .Select(a => a.AccessRole == AccessRoles.Administrator)
            .SingleAsync(ct);

    private FileContentResult IcsResult(string fileName, string calendarName, IReadOnlyList<ScheduleFeedItem> schedules)
    {
        var cal = new Calendar { ProductId = "-//TuitionManagementSystem//Schedules//EN", Method = "PUBLISH" };

        cal.AddProperty("X-WR-CALNAME", calendarName);

        foreach (var s in schedules)
        {
            var ev = s.Schedule.ToICalendarEvent();
            ev.Uid = $"schedule-{s.Schedule.Id}@tms";

            if (!string.IsNullOrWhiteSpace(s.ClassroomLocation))
                ev.Location = s.ClassroomLocation;

            if (string.IsNullOrWhiteSpace(ev.Summary))
                ev.Summary = s.CourseName;

            cal.Events.Add(ev);
        }

        var serializer = new CalendarSerializer();
        var ics = serializer.SerializeToString(cal);

        return File(System.Text.Encoding.UTF8.GetBytes(ics), "text/calendar; charset=utf-8", fileName);
    }


}
public sealed record ScheduleFeedItem(
    TuitionManagementSystem.Web.Models.Class.Schedule Schedule,
    string CourseName,
    string? ClassroomLocation
);
