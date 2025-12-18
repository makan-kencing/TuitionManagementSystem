namespace TuitionManagementSystem.Web.Features.Schedule;

using System.Globalization;
using Course;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TuitionManagementSystem.Web.Features.Course;
using TuitionManagementSystem.Web.ViewModels.Schedule;
using ViewModels.Schedule;


[Route("schedules")]
public sealed class ScheduleController(IMediator mediator) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int? year, int? month, int? courseId)
    {
        var now = DateTime.Now;
        var y = year ?? now.Year;
        var m = month ?? now.Month;

        await LoadCourses(courseId);

        var occ = await mediator.Send(new GetScheduleOccurrencesByMonth(
            CourseId: courseId,
            Year: y,
            Month: m
        ));

        var vm = new ScheduleIndexVm
        {
            Year = y,
            Month = m,
            CourseId = courseId,
            Occurrences = occ
        };
        return View("ScheduleIndex", vm);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        await LoadCourses(null);
        var start = DateTime.Now.Date.AddHours(18);
        return View("ScheduleCreate", new ScheduleFormVm { Start = start, End = start.AddHours(1) });
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ScheduleFormVm vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadCourses(vm.CourseId);
            return View("ScheduleCreate", vm);
        }

        // IMPORTANT: keep wall-clock local in the controller.
        // The handler will validate and convert to UTC for storage.
        vm.Start = NormalizeWallClockLocal(vm.Start);
        vm.End   = NormalizeWallClockLocal(vm.End);
        vm.Until = vm.Until is null ? null : NormalizeWallClockLocal(vm.Until.Value);

        var patterns = BuildPatterns(vm);
        var rdates = ParseDateLines(vm.RecurrenceDatesText);
        var exdates = ParseDateLines(vm.ExceptionDatesText);

        try
        {
            await mediator.Send(new CreateSchedule(
                vm.CourseId, vm.Summary, vm.Description, vm.Start, vm.End,
                patterns, rdates, exdates));
        }
        catch (InvalidOperationException ex)
        {
            // Prevent “unhandled exception” page for validation rules like 09:00–23:00
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadCourses(vm.CourseId);
            return View("ScheduleCreate", vm);
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is Npgsql.PostgresException p &&
            p.SqlState == "23505" &&
            p.ConstraintName == "IX_Schedules_CourseId")
        {
            ModelState.AddModelError(nameof(vm.CourseId),
                "A schedule already exists for this course. Please edit the existing schedule.");

            await LoadCourses(vm.CourseId);
            return View("ScheduleCreate", vm);
        }

        return RedirectToAction(nameof(Index), new
        {
            year = vm.Start.Year,
            month = vm.Start.Month,
            courseId = vm.CourseId
        });
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var s = await mediator.Send(new GetScheduleById(id));
        if (s is null) return NotFound();

        var vm = new ScheduleFormVm
        {
            Id = s.Id,
            CourseId = s.CourseId,
            Summary = s.Summary,
            Description = s.Description,
            Start = s.Start.ToLocalTime(),
            End = s.End.ToLocalTime(),
            RecurrenceDatesText = string.Join(Environment.NewLine,
                s.RecurrenceDates.Select(d => d.ToLocalTime().ToString("yyyy-MM-dd"))),
            ExceptionDatesText = string.Join(Environment.NewLine,
                s.ExceptionDates.Select(d => d.ToLocalTime().ToString("yyyy-MM-dd")))
        };

        var weekly = s.RecurrencePatterns.FirstOrDefault(p => p.FrequencyType == Ical.Net.FrequencyType.Weekly);
        if (weekly is not null)
        {
            vm.Repeat = RepeatKind.Weekly;
            vm.Interval = weekly.Interval <= 0 ? 1 : weekly.Interval;
            vm.ByDay = weekly.ByDay?.ToList() ?? [];
            vm.Until = weekly.Until?.ToLocalTime();
        }

        await LoadCourses(vm.CourseId);
        return View("ScheduleEdit", vm);
    }

    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ScheduleFormVm vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await LoadCourses(vm.CourseId);
            return View("ScheduleEdit", vm);
        }

        // IMPORTANT: keep wall-clock local in the controller.
        vm.Start = NormalizeWallClockLocal(vm.Start);
        vm.End   = NormalizeWallClockLocal(vm.End);
        vm.Until = vm.Until is null ? null : NormalizeWallClockLocal(vm.Until.Value);

        var patterns = BuildPatterns(vm);
        var rdates = ParseDateLines(vm.RecurrenceDatesText);
        var exdates = ParseDateLines(vm.ExceptionDatesText);

        try
        {
            var ok = await mediator.Send(new UpdateSchedule(
                vm.Id, vm.CourseId, vm.Summary, vm.Description, vm.Start, vm.End,
                patterns, rdates, exdates));

            if (!ok) return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadCourses(vm.CourseId);
            return View("ScheduleEdit", vm);
        }

        return RedirectToAction(nameof(Index), new
        {
            year = vm.Start.Year,
            month = vm.Start.Month,
            courseId = vm.CourseId
        });
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await mediator.Send(new DeleteSchedule(id));
        if (!ok) return NotFound();
        return RedirectToAction(nameof(Index));
    }

    private List<RecurrencePatternDto> BuildPatterns(ScheduleFormVm vm)
    {
        if (vm.Repeat == RepeatKind.None) return [];

        var byDay = vm.ByDay?.Distinct().ToList() ?? [];
        if (byDay.Count == 0) byDay = [vm.Start.DayOfWeek];

        return
        [
            new RecurrencePatternDto(
                FrequencyType: Ical.Net.FrequencyType.Weekly,
                Until: vm.Until,
                Count: null,
                Interval: vm.Interval <= 0 ? 1 : vm.Interval,
                ByDay: byDay)
        ];
    }

    private static List<DateTime> ParseDateLines(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return [];

        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var list = new List<DateTime>();

        foreach (var line in lines)
        {
            if (DateTime.TryParseExact(line.Trim(), "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                // Keep date as local wall-clock date.
                list.Add(DateTime.SpecifyKind(dt.Date, DateTimeKind.Local));
            }
        }

        return list;
    }

    [HttpGet("calendar")]
    public IActionResult CalendarAll()
        => View("ScheduleCalendar", new ScheduleCalendarVm());

    [HttpGet("calendar/course/{courseId:int}")]
    public IActionResult CalendarCourse(int courseId)
        => View("ScheduleCalendar", new ScheduleCalendarVm { CourseId = courseId });

    [HttpGet("calendar/teacher/{teacherId:int}")]
    public IActionResult CalendarTeacher(int teacherId)
        => View("ScheduleCalendar", new ScheduleCalendarVm { TeacherId = teacherId });

    [HttpGet("calendar/student/{studentId:int}")]
    public IActionResult CalendarStudent(int studentId)
        => View("ScheduleCalendar", new ScheduleCalendarVm { StudentId = studentId });

    [HttpGet("events")]
    public async Task<IActionResult> Events(
        DateTime start,
        DateTime end,
        int? courseId,
        int? teacherId,
        int? studentId,
        CancellationToken ct)
    {
        // FullCalendar often sends UTC; normalize to local wall-clock before handler converts.
        start = NormalizeWallClockLocal(start);
        end   = NormalizeWallClockLocal(end);

        var occ = await mediator.Send(new GetScheduleOccurrencesByDateRange(
            From: start,
            To: end,
            CourseId: courseId,
            TeacherId: teacherId,
            StudentId: studentId
        ), ct);

        return Ok(occ.Select(x => new
        {
            id = $"{x.ScheduleId}:{x.Start:O}",
            title = $"{x.CourseName} ({x.ClassroomName})",
            start = x.Start,
            end = x.End,
            extendedProps = new
            {
                scheduleId = x.ScheduleId,
                courseId = x.CourseId,
                classroomId = x.ClassroomId
            }
        }));
    }

    private async Task LoadCourses(int? selectedCourseId)
    {
        var courses = await mediator.Send(new GetCourses());
        ViewBag.Courses = courses.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name,
            Selected = selectedCourseId.HasValue && c.Id == selectedCourseId.Value
        }).ToList();
    }

    private static DateTime NormalizeWallClockLocal(DateTime dt) =>
        dt.Kind switch
        {
            DateTimeKind.Local => dt,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dt, DateTimeKind.Local),
            DateTimeKind.Utc => DateTime.SpecifyKind(dt.ToLocalTime(), DateTimeKind.Local),
            _ => DateTime.SpecifyKind(dt, DateTimeKind.Local)
        };
}
