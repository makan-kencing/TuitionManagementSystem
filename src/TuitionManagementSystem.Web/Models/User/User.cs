namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;

public abstract class User
{
    [Key]
    public int Id { get; set; }

    public required int AccountId { get; set; }
    public required Account Account { get; set; }

    public virtual ICollection<File> Files { get; set; } = [];
}
