namespace TuitionManagementSystem.Web.Features.Classroom;

using System.ComponentModel.DataAnnotations;

public class ClassroomFormVM
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Location { get; set; } = string.Empty;

    [Range(10, 40)]
    public int MaxCapacity { get; set; }
}
