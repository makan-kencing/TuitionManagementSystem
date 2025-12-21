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
            var startLocal = o.Period.StartTime?.Value ?? default;
            if (startLocal == default) continue;

            var endLocal = o.Period.EndTime?.Value ?? startLocal + duration;

            if (startLocal >= toUtc) break;
            if (endLocal <= fromUtc) continue;

            list.Add(new ScheduleOccurrence(
                ScheduleId: s.Id,
                CourseId: s.CourseId,
                CourseName: s.Course.Name,
                ClassroomId: s.Course.PreferredClassroom.Id,
                ClassroomName: s.Course.PreferredClassroom.Location,
                Start: startLocal,
                End: endLocal
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

    var candidate = new Schedule
    {
        Start = startUtc,
        End = endUtc,
        RecurrencePatterns = patterns.Select(p => new ScheduleRecurrencePattern
        {
            FrequencyType = p.FrequencyType,
            Interval = p.Interval,
            Until = p.Until,
            Count = p.Count,
            ByDay = p.ByDay.ToList()
        }).ToList(),

        RecurrenceDates = recurrenceDates.ToList(),
        ExceptionDates = exceptionDates.ToList()
    };

    var candidateEvent = candidate.ToICalendarEvent();
    var candidateDuration = endUtc - startUtc;
    var checkStart = new CalDateTime(startUtc);

    var maxCheckDate = startUtc.AddYears(2);

    var otherSchedules = await db.Schedules
        .AsNoTracking()
        .Include(s => s.RecurrencePatterns)
        .Include(s => s.Course)
        .Where(s => s.Id != currentScheduleId &&
                    s.Course.PreferredClassroomId == course.PreferredClassroomId)
        .ToListAsync(ct);

    foreach (var candOcc in candidateEvent.GetOccurrences(checkStart))
    {
        var cStart = candOcc.Period.StartTime.AsUtc;
        if (cStart > maxCheckDate) break;

        var cEnd = candOcc.Period.EndTime?.AsUtc ?? cStart.Add(candidateDuration);

        foreach (var other in otherSchedules)
        {
            var otherEvent = other.ToICalendarEvent();
            var otherDuration = other.End - other.Start;

            foreach (var existingOcc in otherEvent.GetOccurrences(checkStart))
            {
                var oStart = existingOcc.Period.StartTime.AsUtc;
                if (oStart > maxCheckDate) break;

                var oEnd = existingOcc.Period.EndTime?.AsUtc ?? oStart.Add(otherDuration);


                if (cStart < oEnd && cEnd > oStart)
                {
                    throw new InvalidOperationException(
                        $"Classroom Conflict: Room is busy for '{other.Summary}' " +
                        $"on {cStart.ToLocalTime():MMM dd, yyyy} at {cStart.ToLocalTime():HH:mm}.");
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
