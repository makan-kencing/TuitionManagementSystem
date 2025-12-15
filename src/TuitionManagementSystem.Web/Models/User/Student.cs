namespace TuitionManagementSystem.Web.Models.User;

using Class;
using Payment;

public class Student : User
{
    public ICollection<Attendance> Attendances { get; set; } = [];

    public ICollection<Enrollment> Enrollments { get; set; } = [];

    public ICollection<Invoice> Invoices { get; set; } = [];
}
