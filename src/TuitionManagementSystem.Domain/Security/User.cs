namespace TuitionManagementSystem.Domain.Security;

using System.ComponentModel.DataAnnotations;

public abstract class User
{
    public int Id { get; set; }

    public required string Username { get; set; }

    [EmailAddress]
    public required string Email { get; set; }

    [DataType(DataType.Password)]
    public required string HashedPassword { get; set; }
}
