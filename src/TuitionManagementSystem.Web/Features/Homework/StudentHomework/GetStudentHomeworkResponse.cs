namespace TuitionManagementSystem.Web.Features.Homework.StudentHomework;

public class GetStudentHomeworkResponse
{
    public string? Name { get; set; }

    public List<StudentCourseInfo> CourseInfos { get; set; } = [];
}

public class StudentCourseInfo
{
    public int Id { get; set; }

    public DateTime EnrollDate { get; set; }

    public string? Name { get; set; }

    public string? SubjectName { get; set; }

    public required CourseTeacherInfo Teacher { get; set; }
}

public class CourseTeacherInfo
{
    public int Id { get; set; }

    public string? Name { get; set; }
}
