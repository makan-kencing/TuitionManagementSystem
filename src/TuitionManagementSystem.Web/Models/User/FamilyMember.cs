namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(FamilyId), nameof(UserId), IsUnique = true)]
public class FamilyMember
{
    [Key]
    public int Id { get; set; }

    public int FamilyId { get; set; }

    public int UserId { get; set; }

    public DateTime JoinedOn { get; set; } = DateTime.UtcNow;

    public Family Family { get; set; } = null!;

    public User User { get; set; } = null!;
}
