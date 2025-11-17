namespace TuitionManagementSystem.Domain.Entities.Account;

using System.ComponentModel.DataAnnotations;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    [MaxLength(254)]
    public required string Email { get; set; }

    [Required]
    public required string HashedPassword { get; set; }
}
