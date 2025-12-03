namespace TuitionManagementSystem.Web.Models.Class.Announcement;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AnnouncementFile
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Announcement) + "Id")]
    public required Announcement Announcement { get; set; }

    [ForeignKey(nameof(File) + "Id")]
    public required File File { get; set; }
}
