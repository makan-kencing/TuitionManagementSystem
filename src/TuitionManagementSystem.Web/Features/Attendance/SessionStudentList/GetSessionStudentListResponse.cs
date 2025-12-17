namespace TuitionManagementSystem.Web.Features.Attendance.SessionStudentList;

public class GetSessionStudentListResponse
{
    public List<StudentInfo> StudentList { get; set; } = [];

}

public class StudentInfo
{
   public int  StudentId  { get; set; }
   public required string Name  { get; set; }
   public string Status { get; set; } = default!;

}
