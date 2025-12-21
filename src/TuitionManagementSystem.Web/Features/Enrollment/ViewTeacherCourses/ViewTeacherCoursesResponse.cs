namespace TuitionManagementSystem.Web.Features.Enrollment.ViewTeacherCourses;

public class ViewTeacherCoursesResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
}
