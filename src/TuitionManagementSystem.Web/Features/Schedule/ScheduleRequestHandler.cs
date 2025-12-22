namespace TuitionManagementSystem.Web.Features.Schedule;


using System.Globalization;
using System.Linq.Expressions;
using Infrastructure.Persistence;
using Ical.Net.DataTypes;
using MediatR;
using Classroom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Models.Class;

public class ScheduleRequestHandler(ApplicationDbContext db) :
    IRequestHandler<CreateSchedule, ScheduleResponse>,
    IRequestHandler<UpdateSchedule, bool>,
    IRequestHandler<DeleteSchedule, bool>,
    IRequestHandler<GetSchedulesByCourse, IEnumerable<ScheduleResponse>>,
    IRequestHandler<GetScheduleById, ScheduleDetailResponse?>,
    IRequestHandler<GetScheduleOccurrencesByMonth, IEnumerable<ScheduleOccurrence>>,
    IRequestHandler<GetScheduleOccurrencesByDateRange, IReadOnlyList<ScheduleOccurrence>>
{
    private static readonly TimeSpan MinTime = TimeSpan.FromHours(9);
    private static readonly TimeSpan MaxTime = TimeSpan.FromHours(23);


    public async Task<ScheduleResponse> Handle(CreateSchedule request, CancellationToken ct)
    {
        ValidateTimeWindow(request.Start, request.End);

        var startUtc = DateTimeUtc.ToUtcAssumingLocal(request.Start);
        var endUtc = DateTimeUtc.ToUtcAssumingLocal(request.End);

        await EnsureNoClassroomConflict(null, request.CourseId, startUtc, endUtc,
            request.RecurrencePatterns, request.RecurrenceDates ?? [], request.ExceptionDates ?? [], ct);

        var entity = new Schedule
        {
            CourseId = request.CourseId,
            Summary = request.Summary,
            Description = request.Description,
            Start = startUtc,
            End = endUtc,


            RecurrencePatterns = request.RecurrencePatterns.Select(MapPatternUtc).ToList(),
            RecurrenceDates = DateTimeUtc.ToUtcAssumingLocal(
                (request.RecurrenceDates ?? [])
                    .Select(d => NormalizeExceptionOrRDate(d, request.Start))
                    .ToList()),
            ExceptionDates = DateTimeUtc.ToUtcAssumingLocal(
                (request.ExceptionDates ?? [])
                    .Select(d => NormalizeExceptionOrRDate(d, request.Start))
                    .ToList())
        };

        await db.Schedules.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);

        return new ScheduleResponse(entity.Id, entity.CourseId, entity.Summary, entity.Description, entity.Start, entity.End);
    }


    public async Task<bool> Handle(UpdateSchedule request, CancellationToken ct)
    {
        ValidateTimeWindow(request.Start, request.End);
        var startUtc = DateTimeUtc.ToUtcAssumingLocal(request.Start);
        var endUtc = DateTimeUtc.ToUtcAssumingLocal(request.End);

        await EnsureNoClassroomConflict(request.Id, request.CourseId, startUtc, endUtc,
            request.RecurrencePatterns, request.RecurrenceDates ?? [], request.ExceptionDates ?? [], ct);

        var entity = await db.Schedules
            .Include(s => s.RecurrencePatterns)
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (entity is null) return false;

        entity.CourseId = request.CourseId;
        entity.Summary = request.Summary;
        entity.Description = request.Description;
        entity.Start = DateTimeUtc.ToUtcAssumingLocal(request.Start);
        entity.End   = DateTimeUtc.ToUtcAssumingLocal(request.End);
        entity.RecurrenceDates = DateTimeUtc.ToUtcAssumingLocal(
            (request.RecurrenceDates ?? [])
                .Select(d => NormalizeExceptionOrRDate(d, request.Start))
                .ToList());
        entity.ExceptionDates  = DateTimeUtc.ToUtcAssumingLocal(
            (request.ExceptionDates ?? [])
                .Select(d => NormalizeExceptionOrRDate(d, request.Start))
                .ToList());

        entity.RecurrencePatterns.Clear();
        foreach (var rp in request.RecurrencePatterns.Select(MapPatternUtc))
            entity.RecurrencePatterns.Add(rp);

        await db.SaveChangesAsync(ct);
        return true;
    }


    public async Task<bool> Handle(DeleteSchedule request, CancellationToken ct)
    {
        var entity = await db.Schedules.FirstOrDefaultAsync(s => s.Id == request.Id, ct);
        if (entity is null) return false;

        db.Schedules.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }


    public async Task<IEnumerable<ScheduleResponse>> Handle(GetSchedulesByCourse request, CancellationToken ct)
    {
        return await db.Schedules
            .AsNoTracking()
            .Where(s => s.CourseId == request.CourseId)
            .Select(s => new ScheduleResponse(s.Id, s.CourseId, s.Summary, s.Description, s.Start, s.End))
            .ToListAsync(ct);
    }


    public async Task<ScheduleDetailResponse?> Handle(GetScheduleById request, CancellationToken ct)
    {
        var s = await db.Schedules
            .AsNoTracking()
            .Include(x => x.RecurrencePatterns)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (s is null) return null;

        var patterns = s.RecurrencePatterns.Select(p => new RecurrencePatternDto(
            FrequencyType: p.FrequencyType,
            Until: p.Until,
            Count: p.Count,
            Interval: p.Interval <= 0 ? 1 : p.Interval,
            ByDay: p.ByDay.ToList()
        ));

        return new ScheduleDetailResponse(
            s.Id,
            s.CourseId,
            s.Summary,
            s.Description,
            s.Start,
            s.End,
            patterns.ToList(),
            s.RecurrenceDates.ToList(),
            s.ExceptionDates.ToList());
    }



    public async Task<IEnumerable<ScheduleOccurrence>> Handle(GetScheduleOccurrencesByMonth request, CancellationToken ct)
    {
        var fromLocal = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Local);
        var toLocal = fromLocal.AddMonths(1);

        return await Handle(new GetScheduleOccurrencesByDateRange(
            From: fromLocal,
            To: toLocal,
            CourseId: request.CourseId
        ), ct);
    }


    public async Task<IReadOnlyList<ScheduleOccurrence>> Handle(GetScheduleOccurrencesByDateRange request, CancellationToken ct)
    {
    var fromUtc = request.From.Kind == DateTimeKind.Utc
        ? request.From
        : DateTimeUtc.ToUtcAssumingLocal(request.From);

    var toUtc = request.To.Kind == DateTimeKind.Utc
        ? request.To
        : DateTimeUtc.ToUtcAssumingLocal(request.To);

    if (toUtc <= fromUtc) return Array.Empty<ScheduleOccurrence>();

    var schedulesQuery = db.Schedules
        .AsNoTracking()
        .Include(s => s.RecurrencePatterns)
        .Include(s => s.Course).ThenInclude(c => c.PreferredClassroom)
        .Where(s =>
            (s.End > fromUtc)
            || s.RecurrencePatterns.Any(p => p.Until == null || p.Until > fromUtc)
            || s.RecurrenceDates.Count > 0)
        .Where(s => s.Start < toUtc);


    if (request.CourseId.HasValue)
        schedulesQuery = schedulesQuery.Where(s => s.CourseId == request.CourseId.Value);

    if (request.TeacherId.HasValue)
    {
        var teacherId = request.TeacherId.Value;
        schedulesQuery = schedulesQuery.Where(s =>
            db.CourseTeachers.Any(ct2 => ct2.CourseId == s.CourseId && ct2.TeacherId == teacherId));
    }

    if (request.StudentId.HasValue)
    {
        var studentId = request.StudentId.Value;
        schedulesQuery = schedulesQuery.Where(s =>
            db.Enrollments.Any(e2 => e2.CourseId == s.CourseId && e2.StudentId == studentId));
    }

    var schedules = await schedulesQuery.ToListAsync(ct);

    var list = new List<ScheduleOccurrence>(capacity: schedules.Count * 8);

    foreach (var s in schedules)
    {
        var duration = s.End - s.Start;
        if (duration <= TimeSpan.Zero) continue;

        var isRecurring = s.RecurrencePatterns.Count > 0 || s.RecurrenceDates.Count > 0;

        if (!isRecurring)
        {
            if (s.Start < toUtc && s.End > fromUtc)
            {
                list.Add(new ScheduleOccurrence(
                    ScheduleId: s.Id,
                    CourseId: s.CourseId,
                    CourseName: s.Course.Name,
                    ClassroomId: s.Course.PreferredClassroom.Id,
                    ClassroomName: s.Course.PreferredClassroom.Location,
                    Start: s.Start,
                    End: s.End
                ));
            }

            continue;
        }

        var ev = s.ToICalendarEvent();
        var occ = ev.GetOccurrences(startTime: new CalDateTime(fromUtc));

        foreach (var o in occ)
        {
            var startUtcOcc = o.Period.StartTime?.AsUtc ?? default;
            if (startUtcOcc == default) continue;

            var endUtcOcc = o.Period.EndTime?.AsUtc ?? startUtcOcc + duration;

            if (startUtcOcc >= toUtc) break;
            if (endUtcOcc <= fromUtc) continue;

            list.Add(new ScheduleOccurrence(
                ScheduleId: s.Id,
                CourseId: s.CourseId,
                CourseName: s.Course.Name,
                ClassroomId: s.Course.PreferredClassroom.Id,
                ClassroomName: s.Course.PreferredClassroom.Location,
                Start: startUtcOcc,
                End: endUtcOcc
            ));
        }
    }

    return list.OrderBy(x => x.Start).ToList();
    }


    private static ScheduleRecurrencePattern MapPatternUtc(RecurrencePatternDto dto) =>
        new()
        {
            FrequencyType = dto.FrequencyType,
            Until = dto.Until is null ? null : DateTimeUtc.ToUtcAssumingLocal(NormalizeUntil(dto.Until.Value)),
            Count = dto.Count,
            Interval = dto.Interval <= 0 ? 1 : dto.Interval,
            ByDay = dto.ByDay?.ToList() ?? []
        };

    private static DateTime NormalizeExceptionOrRDate(DateTime date, DateTime scheduleStart)
    {

        return date.TimeOfDay == TimeSpan.Zero
            ? date.Date + scheduleStart.TimeOfDay
            : date;
    }

    private static DateTime NormalizeUntil(DateTime until)
    {
        return until.TimeOfDay == TimeSpan.Zero
            ? until.Date.AddDays(1).AddTicks(-1)
            : until;
    }

    public async Task<IReadOnlyList<ScheduleFeedItem>> Handle(GetAllSchedulesForFeed request, CancellationToken ct)
    {
        var schedules = await db.Schedules
            .AsNoTracking()
            .Include(s => s.RecurrencePatterns)
            .Include(s => s.Course).ThenInclude(c => c.PreferredClassroom)
            .ToListAsync(ct);

        return schedules.Select(s => new ScheduleFeedItem(
            Schedule: s,
            CourseName: s.Course.Name,
            ClassroomLocation: s.Course.PreferredClassroom?.Location
        )).ToList();
    }

    private async Task EnsureNoClassroomConflict(
    int? currentScheduleId,
    int courseId,
    DateTime startUtc,
    DateTime endUtc,
    IEnumerable<RecurrencePatternDto> patterns,
    IEnumerable<DateTime> recurrenceDates,
    IEnumerable<DateTime> exceptionDates,
    CancellationToken ct)
{
    var course = await db.Courses
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.Id == courseId, ct);

    if (course?.PreferredClassroomId == null) return;

    var preferredRoomId = course.PreferredClassroomId;

    var existingSchedules = await db.Schedules
        .AsNoTracking()
        .Include(s => s.RecurrencePatterns)
        .Include(s => s.Course)
        .Where(s => s.Id != currentScheduleId)
        .Where(s => s.Course.PreferredClassroomId == preferredRoomId)
        .ToListAsync(ct);

    if (existingSchedules.Count == 0) return;

    var candidate = new Schedule
    {
        Id = currentScheduleId ?? 0,
        CourseId = courseId,
        Start = startUtc,
        End = endUtc,
        RecurrencePatterns = patterns.Select(MapPatternUtc).ToList(),
        RecurrenceDates = DateTimeUtc.ToUtcAssumingLocal(
            recurrenceDates.Select(d => NormalizeExceptionOrRDate(d, startUtc.ToLocalTime())).ToList()),
        ExceptionDates = DateTimeUtc.ToUtcAssumingLocal(
            exceptionDates.Select(d => NormalizeExceptionOrRDate(d, startUtc.ToLocalTime())).ToList())
    };

    var candidateDuration = endUtc - startUtc;
    var candidateEvent = candidate.ToICalendarEvent();

    // 3. Define the Search Window (e.g., check conflicts for the next 2 years only)
    var searchStart = new CalDateTime(startUtc.AddDays(-1));
    var searchEnd   = startUtc.AddYears(2);

    // 4. CRITICAL FIX: Use TakeWhile to stop the loop manually
    // We pass 'searchStart' to skip past events, but use TakeWhile to stop future events.
    var candidateOccurrences = candidateEvent
        .GetOccurrences(searchStart)
        .TakeWhile(o => o.Period.StartTime.AsUtc < searchEnd);

    foreach (var candOcc in candidateOccurrences)
    {
        var cStart = candOcc.Period.StartTime.AsUtc;
        var cEnd = candOcc.Period.EndTime?.AsUtc ?? cStart.Add(candidateDuration);

        foreach (var existing in existingSchedules)
        {
            // Optimization: Skip existing schedules that ended before this occurrence starts
            if (existing.RecurrencePatterns.Count == 0 && existing.End < cStart) continue;

            var existingEvent = existing.ToICalendarEvent();
            var existingDuration = existing.End - existing.Start;

            // Define a narrow window for the existing event check to improve performance
            var checkWindowStart = new CalDateTime(cStart.AddDays(-1));
            var checkWindowEnd = cStart.AddDays(1); // Only check 1 day ahead

            // Check existing occurrences
            var existingOccurrences = existingEvent
                .GetOccurrences(checkWindowStart)
                .TakeWhile(o => o.Period.StartTime.AsUtc < checkWindowEnd);

            foreach (var exOcc in existingOccurrences)
            {
                var oStart = exOcc.Period.StartTime.AsUtc;
                var oEnd = exOcc.Period.EndTime?.AsUtc ?? oStart.Add(existingDuration);

                // INTERSECTION CHECK
                if (cStart < oEnd && cEnd > oStart)
                {
                    var dateStr = cStart.ToLocalTime().ToString("dd MMM yyyy (ddd)", CultureInfo.InvariantCulture);
                    var timeStr = $"{cStart.ToLocalTime():h:mm tt} - {cEnd.ToLocalTime():h:mm tt}";

                    throw new InvalidOperationException(
                        $"Conflict detected! The classroom '{course.PreferredClassroom?.Location}' is already booked by " +
                        $"'{existing.Course.Name}' on {dateStr} at {timeStr}.");
                }
            }
        }
    }
}

    private static void ValidateTimeWindow(DateTime start, DateTime end)
    {
        if (start >= end) throw new InvalidOperationException("Start must be before end.");
        var startLocal = start.ToLocalTime();
        var endLocal = end.ToLocalTime();
        if (startLocal.TimeOfDay < MinTime || endLocal.TimeOfDay > MaxTime)
            throw new InvalidOperationException("Schedules must be between 09:00 and 23:00.");
    }
}
