namespace TuitionManagementSystem.Web.ViewModels.Course;
using System.ComponentModel.DataAnnotations;


public class CourseFormVm
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    [Range(0, 99.99)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Subject is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Subject is required")]
    public int SubjectId { get; set; }

    [Required(ErrorMessage = "Preferred classroom is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Preferred classroom is required")]
    public int PreferredClassroomId { get; set; }
}
