namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User;

public class Enrollment
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Student) + "Id")]
    public required Student Student { get; set; }

    [ForeignKey(nameof(Course) + "Id")]
    public required Course Course { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
}
