namespace TuitionManagementSystem.Web.Features.Course;

using MediatR;

public record CreateCourse(string Name, string? Description, decimal Price, int SubjectId, int PreferredClassroomId) : IRequest<CourseResponse>;
public record UpdateCourse(int Id, string Name, string? Description, decimal Price, int SubjectId, int PreferredClassroomId) : IRequest<bool>;
public record DeleteCourse(int Id) : IRequest<bool>;
public record GetCourses() : IRequest<IEnumerable<CourseResponse>>;
public record GetCourseById(int Id) : IRequest<CourseResponse?>;
public record GetCourseIndexRows() : IRequest<IReadOnlyList<CourseIndexRowResponse>>;
public record UpdateCourseLookupsInline(int CourseId, int SubjectId, int PreferredClassroomId) : IRequest<bool>;
public record UpdateCourseTeacherInline(int CourseId, int? TeacherId) : IRequest<bool>;
public record GetTeacherLookups() : IRequest<IReadOnlyList<TeacherLookupResponse>>;
