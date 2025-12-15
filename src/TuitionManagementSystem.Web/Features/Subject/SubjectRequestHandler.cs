namespace TuitionManagementSystem.Web.Features.Subject;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Models.Class;


public class SubjectRequestHandler(ApplicationDbContext db) :
    IRequestHandler<CreateSubject, SubjectResponse>,
    IRequestHandler<UpdateSubject, bool>,
    IRequestHandler<ArchiveSubject, bool>,
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

    public async Task<bool> Handle(ArchiveSubject request, CancellationToken ct)
    {
        var entity = await db.Subjects.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null) return false;
        entity.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Handle(RestoreSubject request, CancellationToken ct)
    {
        var entity = await db.Subjects.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null) return false;
        entity.DeletedAt = null;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<SubjectResponse>> Handle(GetSubjects request, CancellationToken ct)
    {
        // 1. Fetch from DB
        var entities = await db.Subjects
            .AsNoTracking()
            .Where(s => s.DeletedAt == null)
            .ToListAsync(ct);

        // 2. Convert in Memory
        return entities.Select(SubjectResponse.FromEntity);
    }

    public async Task<SubjectResponse?> Handle(GetSubjectById request, CancellationToken ct)
    {
        // 1. Fetch from DB
        var entity = await db.Subjects
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .FirstOrDefaultAsync(ct);

        // 2. Convert in Memory
        return entity is null ? null : SubjectResponse.FromEntity(entity);
    }
}
