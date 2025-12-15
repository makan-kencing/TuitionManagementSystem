namespace TuitionManagementSystem.Web.Features.Classroom;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;

public class ClassroomRequestHandler(ApplicationDbContext db) :
    IRequestHandler<CreateClassroom, ClassroomResponse>,
    IRequestHandler<UpdateClassroom, bool>,
    IRequestHandler<DeleteClassroom, bool>,
    IRequestHandler<GetClassrooms, IEnumerable<ClassroomResponse>>,
    IRequestHandler<GetClassroomById, ClassroomResponse?>
{
    public async Task<ClassroomResponse> Handle(CreateClassroom request, CancellationToken ct)
    {
        var entity = new Classroom
        {
            Location = request.Location.Trim(),
            MaxCapacity = request.MaxCapacity
        };

        await db.Classrooms.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return ClassroomResponse.FromEntity(entity);
    }

    public async Task<bool> Handle(UpdateClassroom request, CancellationToken ct)
    {
        var entity = await db.Classrooms.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity is null) return false;

        var hasChanges = false;

        if (!string.Equals(entity.Location, request.Location, StringComparison.Ordinal))
        {
            entity.Location = request.Location.Trim();
            hasChanges = true;
        }

        if (entity.MaxCapacity != request.MaxCapacity)
        {
            entity.MaxCapacity = request.MaxCapacity;
            hasChanges = true;
        }

        if (!hasChanges) return true;

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Handle(DeleteClassroom request, CancellationToken ct)
    {
        var entity = await db.Classrooms.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity is null) return false;
        db.Classrooms.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<ClassroomResponse>> Handle(GetClassrooms request, CancellationToken ct)
    {
        var entities = await db.Classrooms
            .AsNoTracking()
            .ToListAsync(ct);
        return entities.Select(ClassroomResponse.FromEntity);
    }



    public async Task<ClassroomResponse?> Handle(GetClassroomById request, CancellationToken ct)
    {
        var entity = await db.Classrooms
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        return entity == null ? null : ClassroomResponse.FromEntity(entity);
    }


}
