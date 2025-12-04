namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;

[Index(nameof(SessionId), IsUnique = true)]
public class AccountSession
{
    [Key]
    public int Id { get; set; }

    public Guid SessionId { get; set; } = Guid.NewGuid();

    public IPAddress? LastIp { get; set; }

    public required DateTime LastLogin { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(Account) + "Id")]
    public required Account Account { get; set; }
}
