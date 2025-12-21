namespace TuitionManagementSystem.Web.Features.Dashboard.TeacherDashboard
{
    using System.Collections.Generic;

    public class TeacherDashboardResponse
    {
        public int TotalSessionsToday { get; set; }
        public int ClassesToTeachToday { get; set; }
        public int TotalStudents { get; set; }
        public int HomeworkPending { get; set; }
        public Dictionary<string, int> AttendancePerSession { get; set; }
        public Dictionary<string, int> SubmissionPerCourse { get; set; }

        public Dictionary<string, int> AverageAttendancePerCourse { get; set; }
        public Dictionary<string, int> CourseCapacity { get; set; }
        public Dictionary<string, int> AttendancePercentagePerCourse { get; set; }
    }
}
