namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.EntityFrameworkCore;

[Index(nameof(SessionId), IsUnique = true)]
public class AccountSession
{
    [Key]
    public int Id { get; set; }

    public Guid SessionId { get; set; } = Guid.NewGuid();

    public IPAddress? LastIp { get; set; }

    public DateTime LastLogin { get; set; } = DateTime.UtcNow;

    public int AccountId { get; set; }

    public Account Account { get; set; } = null!;
}
