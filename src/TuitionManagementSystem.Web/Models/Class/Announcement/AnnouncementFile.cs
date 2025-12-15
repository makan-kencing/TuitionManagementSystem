namespace TuitionManagementSystem.Web.Models.Class.Announcement;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(AnnouncementId), nameof(FileId), IsUnique = true)]
public class AnnouncementFile
{
    [Key]
    public int Id { get; set; }

    public int AnnouncementId { get; set; }

    public int FileId { get; set; }

    public Announcement Announcement { get; set; } = null!;

    public File File { get; set; } = null!;
}
