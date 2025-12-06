namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Notification;

public abstract class User
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Account) + "Id")]
    public required Account Account { get; set; }

    public virtual ICollection<File> Files { get; set; } = [];

    public virtual ICollection<Notification> Notifications { get; set; } = [];
}
