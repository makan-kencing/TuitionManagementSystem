namespace TuitionManagementSystem.Web.Features.Attendance.SessionStudentList;

public class GetSessionStudentListResponse
{
    public List<StudentInfo> StudentList { get; set; } = [];
    public  int SessionId { get; set; }

    public bool IsCodeGenerated { get; set; }

}

public class StudentInfo
{

   public int  StudentId  { get; set; }
   public required string Name  { get; set; }

   public int? AttendanceId { get; set; }
   public string Status { get; set; } = default!;
   public DateTime StartDate { get; set; }
   public DateTime EndDate { get; set; }

   public DateTime CreatedOn { get; set; }

}
