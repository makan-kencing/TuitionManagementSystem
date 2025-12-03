namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Features.Authentication.Constants;

public class Account
{
    [Key]
    public int Id { get; set; }

    [StringLength(30)]
    public required string Username { get; set; }

    [StringLength(300)]
    public required string HashedPassword { get; set; }

    public required AccessRoles AccessRole { get; set; } = AccessRoles.User;

    [MaxLength(254)]
    public required string? Email { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public required DateTime LastChanged { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }

    public virtual User? User { get; set; }
}
