namespace TuitionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;

public class File
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public required string FileName { get; set; }

    [StringLength(127)]
    public required string MimeType { get; set; }

    [StringLength(500)]
    public required string Uri { get; set; }

    [StringLength(255)] public string? CanonicalPath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? CreatedById { get; set; }

    public User.User? CreatedBy { get; set; }
}
