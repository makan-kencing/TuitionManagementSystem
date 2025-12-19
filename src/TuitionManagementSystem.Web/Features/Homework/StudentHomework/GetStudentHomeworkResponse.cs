namespace TuitionManagementSystem.Web.Features.Homework.StudentHomework;

public class GetStudentHomeworkResponse
{
    public string? Name { get; set; }
    public List<StudentCoursesInfo> CourseInfos { get; set; } = [];
    public List<StudentEnrollInfo> EnrollInfos { get; set; } = [];
}

public class StudentEnrollInfo
{
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
}

public class StudentCoursesInfo
{
    public string? CourseName { get; set; }
    public int CourseId { get; set; }
    public string?SubjectName { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public List<StudentCourseSection> CourseSections { get; set; } = [];
}


public class StudentCourseSection
{
    public int SessionId { get; set; }
    public DateTime StratAt { get; set; }
    public DateTime EndAt { get; set; }

}
