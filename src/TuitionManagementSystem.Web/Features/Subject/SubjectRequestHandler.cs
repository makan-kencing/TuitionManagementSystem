namespace TuitionManagementSystem.Web.Features.Subject;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Models.Class;


public class SubjectRequestHandler(ApplicationDbContext db) :
    IRequestHandler<CreateSubject, SubjectResponse>,
    IRequestHandler<UpdateSubject, bool>,
    IRequestHandler<DeleteSubject, bool>,
    IRequestHandler<GetSubjects, IEnumerable<SubjectResponse>>,
    IRequestHandler<GetSubjectById, SubjectResponse?>
{
    public async Task<SubjectResponse> Handle(CreateSubject request, CancellationToken ct)
    {
        var entity = new Subject { Name = request.Name, Description = request.Description };
        db.Subjects.Add(entity);
        await db.SaveChangesAsync(ct);
        return SubjectResponse.FromEntity(entity);
    }

    public async Task<bool> Handle(UpdateSubject request, CancellationToken ct)
    {
        var entity = await db.Subjects.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null) return false;
        entity.Name = request.Name;
        entity.Description = request.Description;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Handle(DeleteSubject request, CancellationToken ct)
    {
        var entity = await db.Subjects.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null) return false;
        db.Subjects.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<SubjectResponse>> Handle(GetSubjects request, CancellationToken ct)
        => await db.Subjects
            .AsNoTracking()
            .Select(s => SubjectResponse.FromEntity(s))
            .ToListAsync(ct);

    public async Task<SubjectResponse?> Handle(GetSubjectById request, CancellationToken ct)
        => await db.Subjects
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .Select(s => SubjectResponse.FromEntity(s))
            .FirstOrDefaultAsync(ct);
}
