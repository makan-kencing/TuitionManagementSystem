namespace TuitionManagementSystem.Web.Features.Classroom;
using Models.Class;
public record ClassroomResponse(int Id, string Location, int MaxCapacity)
{
    public static ClassroomResponse FromEntity(Classroom c) =>
        new(c.Id, c.Location, c.MaxCapacity);

}
