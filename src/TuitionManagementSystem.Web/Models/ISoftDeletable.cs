namespace TuitionManagementSystem.Web.Models;

public interface ISoftDeletable
{
    public DateTime? DeletedAt { get; }
}
