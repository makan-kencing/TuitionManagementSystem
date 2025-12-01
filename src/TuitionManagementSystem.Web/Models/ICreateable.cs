namespace TuitionManagementSystem.Web.Models;

public interface ICreateable
{
    public User.User CreatedBy { get; }
    public DateTime CreatedAt { get; }
}
