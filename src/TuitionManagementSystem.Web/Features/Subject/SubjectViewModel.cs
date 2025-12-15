using System.ComponentModel.DataAnnotations;

namespace TuitionManagementSystem.Web.ViewModels.Subject;

public class SubjectFormVm
{
    public int id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }
}


