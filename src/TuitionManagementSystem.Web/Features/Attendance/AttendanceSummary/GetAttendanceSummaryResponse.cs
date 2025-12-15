namespace TuitionManagementSystem.Web.Features.Attendance.AttendanceSummary;

public class GetAttendanceSummaryResponse
{
    public required StudentInfo Student { get; set; }

    public ICollection<CourseAttendanceSummary> Courses { get; set; } = [];

    public AttendanceStats Stats => this.Courses
        .GroupBy(c => c.Course)
        .Select(c => new AttendanceStats(
            c.Sum(s => s.Stats.TotalSessions),
            c.Sum(s => s.Stats.AttendedSessions)
        ))
        .First();
}

public class CourseAttendanceSummary
{
    public required CourseInfo Course { get; set; }

    public required AttendanceStats Stats { get; set; }
}

public record StudentInfo(
    int Id,
    string Name
);

public record CourseInfo(
    int Id,
    string Name
);

public record AttendanceStats(
    int TotalSessions,
    int AttendedSessions
)
{
    public double AttendanceRate => (double)this.AttendedSessions / this.TotalSessions;
}
