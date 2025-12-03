namespace TuitionManagementSystem.Web.Models.User;

using Class;
using Payment;

public class Student : User, IHasFamily
{
    public int? FamilyId { get; set; }
    public Family? Family { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = [];

    public virtual ICollection<Enrollment> Enrollments { get; set; } = [];

    public virtual ICollection<Invoice> Invoices { get; set; } = [];
}
