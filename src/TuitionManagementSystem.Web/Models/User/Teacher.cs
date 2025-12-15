namespace TuitionManagementSystem.Web.Models.User;

using Class;

public class Teacher : User
{
    public ICollection<CourseTeacher> Courses { get; set; } = [];
}
