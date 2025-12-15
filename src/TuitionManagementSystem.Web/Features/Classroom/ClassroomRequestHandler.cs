namespace TuitionManagementSystem.Web.Features.Classroom;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;

public class ClassroomRequestHandler :
    IRequestHandler<CreateClassroom, ClassroomResponse>,
    IRequestHandler<UpdateClassroom, bool>,
    IRequestHandler<DeleteClassroom, bool>,
    IRequestHandler<GetClassrooms, IEnumerable<ClassroomResponse>>,
    IRequestHandler<GetClassroomById, ClassroomResponse?>
{
    private readonly ApplicationDbContext _db;
    public ClassroomRequestHandler(ApplicationDbContext db) => _db = db;

    public async Task<ClassroomResponse> Handle(CreateClassroom request, CancellationToken ct)
    {
        var entity = new Classroom { Location = request.Location, MaxCapacity = request.MaxCapacity };
        _db.Classrooms.Add(entity);
        await _db.SaveChangesAsync(ct);
        return ClassroomResponse.FromEntity(entity);
    }

    public async Task<bool> Handle(UpdateClassroom request, CancellationToken ct)
    {
        var entity = await _db.Classrooms.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity is null) return false;
        entity.Location = request.Location;
        entity.MaxCapacity = request.MaxCapacity;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Handle(DeleteClassroom request, CancellationToken ct)
    {
        var entity = await _db.Classrooms.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity is null) return false;
        _db.Classrooms.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<ClassroomResponse>> Handle(GetClassrooms request, CancellationToken ct)
    {
        // 1. Fetch data from DB first
        var entities = await _db.Classrooms
            .AsNoTracking()
            .ToListAsync(ct);

        // 2. Convert to Response in memory
        return entities.Select(ClassroomResponse.FromEntity);
    }



    public async Task<ClassroomResponse?> Handle(GetClassroomById request, CancellationToken ct)
    {
        // 1. Find the entity in the DB
        var entity = await _db.Classrooms
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        // 2. If found, convert it; otherwise return null
        return entity == null ? null : ClassroomResponse.FromEntity(entity);
    }


}
