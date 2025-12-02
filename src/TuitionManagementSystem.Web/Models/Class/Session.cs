namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;

public class Session
{
    [Key]
    public int Id { get; set; }

    public required Course Course { get; set; }

    public required DateTime StartAt { get; set; }

    public required DateTime EndAt { get; set; }
}
