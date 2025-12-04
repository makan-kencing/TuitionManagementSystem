namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;

[Index(nameof(SessionId))]
public class AccountSession
{
    [Key]
    public int Id { get; set; }

    public required string SessionId { get; set; }

    public IPAddress? LastIp { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public required DateTime LastLogin { get; set; }

    [ForeignKey(nameof(Account) + "Id")]
    public required Account Account { get; set; }
}
