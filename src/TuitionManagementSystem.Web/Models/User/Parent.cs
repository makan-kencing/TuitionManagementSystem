namespace TuitionManagementSystem.Web.Models.User;

public class Parent : User, IHasFamily
{
    public int? FamilyId { get; set; }
    public Family? Family { get; set; }
}
