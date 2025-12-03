namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;

public class Session
{
    [Key]
    public int Id { get; set; }

    public required int CourseId { get; set; }
    public required Course Course { get; set; }

    public required int ClassroomId { get; set; }
    public required Classroom Classroom { get; set; }

    public required DateTime StartAt { get; set; }

    public required DateTime EndAt { get; set; }
}
