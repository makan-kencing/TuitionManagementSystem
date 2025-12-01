namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using User;

public class Enrollment
{
    [Key]
    public int Id { get; set; }

    public required Student Student { get; set; }

    public required Class Class { get; set; }

    public DateTime EnrolledAt { get; set; }= DateTime.Now;
}
