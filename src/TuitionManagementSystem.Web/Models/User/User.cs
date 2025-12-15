namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Notification;

[Index(nameof(Account) + "Id", IsUnique = true)]
public abstract class User
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Family) + "Id")]
    public Family? Family { get; set; }

    [ForeignKey(nameof(Account) + "Id")]
    public required Account Account { get; set; }

    public virtual ICollection<File> Files { get; set; } = [];

    public virtual ICollection<Notification> Notifications { get; set; } = [];
}
