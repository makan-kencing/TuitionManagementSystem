namespace TuitionManagementSystem.Web.Features.Subject;
using MediatR;


public record CreateSubject(string Name, string? Description) : IRequest<SubjectResponse>;
public record UpdateSubject(int Id, string Name, string? Description) : IRequest<bool>;
public record DeleteSubject(int Id) : IRequest<bool>;
public record GetSubjects() : IRequest<IEnumerable<SubjectResponse>>;
public record GetSubjectById(int Id) : IRequest<SubjectResponse?>;
