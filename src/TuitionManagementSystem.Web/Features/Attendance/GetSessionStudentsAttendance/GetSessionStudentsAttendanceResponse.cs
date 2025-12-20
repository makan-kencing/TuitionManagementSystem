namespace TuitionManagementSystem.Web.Features.Attendance.GetSessionStudentsAttendance;

public class GetSessionStudentsAttendanceResponse
{
    public List<StudentInfo> StudentList { get; set; } = [];
    public  int SessionId { get; set; }

    public bool IsCodeGenerated { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool CanTakeAttendance =>
        this.StartDate <= this.CreatedOn
        && this.CreatedOn <= this.EndDate
        && this.IsCodeGenerated;
}

public class StudentInfo
{
   public int  StudentId  { get; set; }

   public string? DisplayName  { get; set; }

   public int? AttendanceId { get; set; }
}
