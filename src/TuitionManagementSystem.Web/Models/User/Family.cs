namespace TuitionManagementSystem.Web.Models.User;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Family
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public required string Name { get; set; }

    public ICollection<FamilyMember> Members { get; set; } = [];

    [NotMapped]
    public IQueryable<FamilyMember> Parents => this.Members.AsQueryable()
        .Where(m => m.User is Parent);

    [NotMapped]
    public IQueryable<FamilyMember> Children => this.Members.AsQueryable()
        .Where(m => m.User is Student);
}
