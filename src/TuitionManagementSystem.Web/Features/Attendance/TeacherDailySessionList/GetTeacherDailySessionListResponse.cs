namespace TuitionManagementSystem.Web.Features.Attendance.TeacherDailySessionList;

public class GetTeacherDailySessionListResponse
{
    public ICollection<CourseDaily>  Courses { get; set; } = [];
}

public class CourseDaily
{
     public required string Name { get; set; }

     public ICollection<SessionDaily> Sessions { get; set; } = [];
}

public class SessionDaily
{
    public int Id { get; set; }

    public required DateTime StartAt { get; set; }

    public required DateTime EndAt { get; set; }

    public string? Code { get; set; }
}


