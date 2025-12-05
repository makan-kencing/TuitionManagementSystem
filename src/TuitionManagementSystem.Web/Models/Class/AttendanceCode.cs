namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Index(nameof(DatedFor), nameof(Code), IsUnique = true)]
public class AttendanceCode
{
    [Key]
    public int Id { get; set; }

    [StringLength(6)]
    [RegularExpression(@"^\d{6}$")]
    public required string Code { get; set; }

    [ForeignKey(nameof(Session) + "Id")]
    public required Session Session { get; set; }

    public required DateOnly DatedFor { get; set; }
}
