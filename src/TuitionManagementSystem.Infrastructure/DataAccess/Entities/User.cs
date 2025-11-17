namespace TuitionManagementSystem.Infrastructure.DataAccess.Entities;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[MetadataType(typeof(UserMetadata))]
public class User(int Id, string Username, string Email, string HashedPassword) : Domain.Security.User
{
}

[Index(nameof(Username), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
internal class UserMetadata
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required, MaxLength(254)]
    public required string Email { get; set; }

    [Required]
    public required string HashedPassword { get; set; }
}
