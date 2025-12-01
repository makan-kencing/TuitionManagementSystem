namespace TuitionManagementSystem.Web.Models.User;

using Class;
using Payment;

public class Student : User
{
    public Family? Family { get; set; }

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
