namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Notification;

[Index(nameof(AccountId), IsUnique = true)]
public abstract class User
{
    [Key]
    public int Id { get; set; }

    public int AccountId { get; set; }

    public Account Account { get; set; } = null!;

    public FamilyMember? Family { get; set; }

    public ICollection<File> Files { get; set; } = [];

    public ICollection<Notification> Notifications { get; set; } = [];
}
