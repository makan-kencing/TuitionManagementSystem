namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations.Schema;
using Class;
using Payment;

public class Student : User, IHasFamily
{
    [ForeignKey(nameof(Family) + "Id")]
    public Family? Family { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = [];

    public virtual ICollection<Enrollment> Enrollments { get; set; } = [];

    public virtual ICollection<Invoice> Invoices { get; set; } = [];
}
