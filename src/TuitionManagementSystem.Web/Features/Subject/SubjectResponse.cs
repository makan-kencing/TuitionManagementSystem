using TuitionManagementSystem.Web.Models.Class;

public record SubjectResponse(int Id, string Name, string? Description)
{
    public static SubjectResponse FromEntity(Subject s)
        => new(s.Id, s.Name, s.Description);
}
