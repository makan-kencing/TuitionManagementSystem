namespace TuitionManagementSystem.Web.Models.Class;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Code), IsUnique = true)]
public class AttendanceCode
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; set; }

    [StringLength(6)]
    [RegularExpression(@"^\d{6}$")]
    public required string Code { get; set; }

    public int? SessionId { get; set; }

    public Session? Session { get; set; }
}
