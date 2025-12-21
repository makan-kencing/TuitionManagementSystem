namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Services.Auth.Constants;

[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class Account
{
    [Key]
    public int Id { get; set; }

    [StringLength(30)]
    public required string Username { get; set; }

    [StringLength(50)]
    public string? DisplayName { get; set; }

    [MaxLength(254)]
    public required string? Email { get; set; }

    [StringLength(300)]
    public required string HashedPassword { get; set; }

    public AccessRoles AccessRole { get; set; } = AccessRoles.User;

    public int? ProfileImageId { get; set; }

    public File? ProfileImage { get; set; }

    public DateTime LastChanged { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }

    public User? User { get; set; }

    public bool IsTwoFactorEnabled { get; set; } = false;

    [StringLength(100)]
    public string? TwoFactorToken { get; set; }

    public DateTime? TwoFactorTokenExpiry { get; set; }

    public ICollection<AccountSession> Sessions { get; set; } = [];

    [NotMapped] public string Name => this.DisplayName ?? this.Username;
}
