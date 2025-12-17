namespace TuitionManagementSystem.Web.Features.Course;

using MediatR;

public record CourseResponse(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int SubjectId,
    string SubjectName,
    int PreferredClassroomId,
    string PreferredClassroomLocation)

{
    public static CourseResponse FromEntity(Models.Class.Course c) =>
        new(
            c.Id,
            c.Name,
            c.Description,
            c.Price,
            c.Subject.Id,
            c.Subject.Name,
            c.PreferredClassroom.Id,
            c.PreferredClassroom.Location);

}

public record CourseIndexRowResponse(
    int Id,
    string Name,
    decimal Price,
    int SubjectId,
    string SubjectName,
    int PreferredClassroomId,
    string PreferredClassroomLocation,
    string TeacherNames,
    int? TeacherId
);

public record TeacherLookupResponse(
    int Id,
    string FullName
);
