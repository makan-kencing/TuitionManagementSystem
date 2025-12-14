namespace TuitionManagementSystem.Web.Features.Attendance.AttendanceSummary;

using Ardalis.Result;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class GetAttendanceSummaryRequestHandler(
    ApplicationDbContext db) : IRequestHandler<GetAttendanceSummaryRequest, Result<GetAttendanceSummaryResponse>>
{
    public async Task<Result<GetAttendanceSummaryResponse>> Handle(
        GetAttendanceSummaryRequest request,
        CancellationToken cancellationToken)
    {
        // motherfucker ef core is so the bane of my fucking life
        // var summary = await db.Enrollments
        //     .Where(e => e.Student.Id == request.StudentId)
        //     .GroupBy(e => e.Student)
        //     .Select(g => new GetAttendanceSummaryResponse
        //     {
        //         Student = new StudentInfo(g.Key.Id, g.Key.Account.Username),
        //         Courses = g.GroupBy(e => e.Course)
        //             .Select(g => new CourseAttendanceSummary
        //             {
        //                 Course = new CourseInfo(g.Key.Id, g.Key.Name),
        //                 Stats = new AttendanceStats(
        //                     g.SelectMany(e => e.Course.Sessions).Count(),
        //                     g.SelectMany(e => e.Course.Sessions).SelectMany(s => s.Attendances).Count()
        //                 )
        //             }).ToList()
        //     }).FirstOrDefaultAsync(cancellationToken);

        var courseSummary = await db.Database
            .SqlQuery<CourseQuery>
            ($"""
              SELECT C."Id", C."Name", Count(S."Id") "Total", Count(Att."Id") "Attended"
              FROM "Enrollments"
                       JOIN public."Courses" C on "Enrollments"."CourseId" = C."Id"
                       LEFT JOIN public."Sessions" S on C."Id" = S."CourseId"
                       LEFT JOIN public."Attendances" Att on S."Id" = Att."SessionId"
              WHERE "Enrollments"."StudentId" = {request.StudentId}
              GROUP BY C."Id", C."Name"
              """)
            .Select(r => r.ToSummary)
            .ToListAsync(cancellationToken);

        var fullSummary = await db.Students
            .Where(s => s.Id == request.StudentId)
            .Select(s => new GetAttendanceSummaryResponse
                {
                    Student = new StudentInfo(s.Id, s.Account.Username), Courses = courseSummary
                }
            )
            .FirstOrDefaultAsync(cancellationToken);

        return fullSummary is not null
            ? Result.Success(fullSummary)
            : Result.NotFound();
    }

    private record CourseQuery(
        int Id,
        string Name,
        int Total,
        int Attended)
    {
        public CourseAttendanceSummary ToSummary =>
            new()
            {
                Course = new CourseInfo(this.Id, this.Name), Stats = new AttendanceStats(this.Total, this.Attended)
            };
    }
}
