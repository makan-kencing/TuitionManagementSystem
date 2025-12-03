namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User;

public class Enrollment
{
    [Key]
    public int Id { get; set; }

    public required int StudentId { get; set; }
    public required Student Student { get; set; }

    public required int CourseId { get; set; }
    public required Course Course { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public required DateTime EnrolledAt { get; set; } = DateTime.Now;
}
