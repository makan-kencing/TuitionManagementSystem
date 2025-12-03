namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User;

public class CourseTeacher
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Course) + "Id")]
    public required Course Course { get; set; }

    [ForeignKey(nameof(Teacher) + "Id")]
    public required Teacher Teacher { get; set; }
}
