namespace TuitionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public required string Username { get; set; }

    [Required]
    [MaxLength(254)]
    public required string Email { get; set; }

    [Required]
    [MaxLength(300)]
    public required string HashedPassword { get; set; }

    [Required]
    public DateTime LastChanged { get; set; } = DateTime.UtcNow;
}
