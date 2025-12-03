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

    public required bool IsLocal { get; set; }

    [ForeignKey(nameof(CreatedBy) + "Id")]
    public User.User? CreatedBy { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public required DateTime CreatedAt { get; set; } = DateTime.Now;
}
