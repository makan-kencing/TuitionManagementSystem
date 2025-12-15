namespace TuitionManagementSystem.Web.Features.Subject;
using Models.Class;


public record SubjectResponse(int Id, string Name, string? Description, DateTime? DeletedAt)
{
    public bool IsArchived => DeletedAt != null;
    public static SubjectResponse FromEntity(Subject s)
        => new(s.Id, s.Name, s.Description, s.DeletedAt);
}
