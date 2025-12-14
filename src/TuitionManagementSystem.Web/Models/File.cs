namespace TuitionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class File
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public required string FileName { get; set; }

    [StringLength(127)]
    public required string MimeType { get; set; }

    public required Uri Uri { get; set; }

    [StringLength(255)]
    public string? CanonicalPath { get; set; }

    [ForeignKey(nameof(CreatedBy) + "Id")]
    public User.User? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
