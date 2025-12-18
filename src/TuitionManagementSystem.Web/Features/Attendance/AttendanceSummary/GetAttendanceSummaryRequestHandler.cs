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
              SELECT C."Id", C."Name" "CourseName", S."Name" "SubjectName", Count(SS."Id") "Total", Count(Att."Id") "Attended"
              FROM "Enrollments" E
                       JOIN public."Courses" C on E."CourseId" = C."Id"
                       JOIN public."Subjects" S on C."SubjectId" = S."Id"
                       LEFT JOIN public."Sessions" SS on C."Id" = SS."CourseId"
                       LEFT JOIN public."Attendances" Att on SS."Id" = Att."SessionId" AND E."StudentId"=Att."StudentId"
              WHERE E."StudentId" = {request.UserId}
              GROUP BY C."Id", C."Name", S."Name"
              """)
            .Select(r => r.ToSummary)
            .ToListAsync(cancellationToken);

        var fullSummary = await db.Students
            .Where(s => s.Id == request.UserId)
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
        string CourseName,
        string SubjectName,
        int Total,
        int Attended)
    {
        public CourseAttendanceSummary ToSummary =>
            new()
            {
                Course = new CourseInfo(this.Id, this.CourseName, this.SubjectName),
                Stats = new AttendanceStats(this.Total, this.Attended)
            };
    }
}
