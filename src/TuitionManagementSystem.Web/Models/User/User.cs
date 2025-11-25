namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Features.Authentication.Constants;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public required string Username { get; set; }

    [Required]
    [MaxLength(300)]
    public required string HashedPassword { get; set; }

    [Required]
    public required AccessRoles AccessRole { get; set; } = AccessRoles.User;

    [MaxLength(254)]
    public required string? Email { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime LastChanged { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }
}
