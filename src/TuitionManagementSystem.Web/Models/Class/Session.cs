namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;

public class Session
{
    [Key]
    public int Id { get; set; }

    public required Class Class { get; set; }

    public required DateTime StartAt { get; set; }

    public required DateTime EndAt { get; set; }
}
