namespace TuitionManagementSystem.Web.Features.Classroom;

using System.ComponentModel.DataAnnotations;

public class ClassroomFormVM
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Location { get; set; } = string.Empty;

    [Range(10, int.MaxValue)]
    public int MaxCapacity { get; set; }
}
