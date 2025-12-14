namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Session
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Course) + "Id")]
    public required Course Course { get; set; }

    [ForeignKey(nameof(Classroom) + "Id")]
    public required Classroom Classroom { get; set; }

    public required DateTime StartAt { get; set; }

    public required DateTime EndAt { get; set; }

    [ForeignKey(nameof(Code) + "Id")]
    public AttendanceCode? Code { get; set; }

    public DateTime? CodeGeneratedAt { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = [];
}
