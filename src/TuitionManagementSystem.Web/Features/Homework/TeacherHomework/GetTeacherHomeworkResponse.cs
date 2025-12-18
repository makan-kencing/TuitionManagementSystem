namespace TuitionManagementSystem.Web.Features.Homework.TeacherHomework;

public class GetTeacherHomeworkResponse
{
    public TeacherInfo ? TeacherInfo { get; set; }
    public List<CourseInfo> CourseInfos { get; set; } = [];
}

public class TeacherInfo
{
    public string? TeacherName{get;set;}
    public int TeacherId{get;set;}
}

public class CourseInfo
{
    public string? CourseName{get;set;}
    public int CourseId{get;set;}
    public string? SubjectName{get;set;}

}
