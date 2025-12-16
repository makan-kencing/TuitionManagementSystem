namespace TuitionManagementSystem.Web.Features.Attendance.TeacherDailySessionList;

public class GetTeacherDailySessionListResponse
{
    public List<SessionDaily>  Sessions { get; set; } = [];
    public List<CourseDaily>  Courses { get; set; } = [];


}

public class CourseDaily
{
     public string Course { get; set; }
}

public class SessionDaily
{

    public int SessionId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
}


