namespace TuitionManagementSystem.Web.Features.Classroom;

using MediatR;

public record CreateClassroom(string Location, int MaxCapacity) : IRequest<ClassroomResponse>;
public record UpdateClassroom(int Id, string Location, int MaxCapacity) : IRequest<bool>;
public record DeleteClassroom(int Id) : IRequest<bool>;
public record GetClassrooms() : IRequest<IEnumerable<ClassroomResponse>>;
public record GetClassroomById(int Id) : IRequest<ClassroomResponse?>;

