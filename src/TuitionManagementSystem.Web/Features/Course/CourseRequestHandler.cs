namespace TuitionManagementSystem.Web.Features.Course;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;

public class CourseRequestHandler(ApplicationDbContext db) :
    IRequestHandler<CreateCourse, CourseResponse>,
    IRequestHandler<UpdateCourse, bool>,
    IRequestHandler<DeleteCourse, bool>,
    IRequestHandler<GetCourses, IEnumerable<CourseResponse>>,
    IRequestHandler<GetCourseById, CourseResponse?>
{
    public async Task<CourseResponse> Handle(CreateCourse request, CancellationToken ct)
    {
        var subject = await db.Subjects.FirstOrDefaultAsync(s => s.Id == request.SubjectId, ct);
        if (subject is null) throw new InvalidOperationException("Subject not found");

        var classroom = await db.Classrooms.FirstOrDefaultAsync(c => c.Id == request.PreferredClassroomId, ct);
        if (classroom is null) throw new InvalidOperationException("Classroom not found");

        var entity = new Course
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            Price = request.Price,
            Subject = subject,
            PreferredClassroom = classroom
        };

        await db.Courses.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);

        // Reload needed navs (EF may already have them tracked)
        await db.Entry(entity).Reference(e => e.Subject).LoadAsync(ct);
        await db.Entry(entity).Reference(e => e.PreferredClassroom).LoadAsync(ct);

        return CourseResponse.FromEntity(entity);
    }

    public async Task<bool> Handle(UpdateCourse request, CancellationToken ct)
    {
        var entity = await db.Courses
            .Include(c => c.Subject)
            .Include(c => c.PreferredClassroom)
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (entity is null) return false;

        var subject = await db.Subjects.FirstOrDefaultAsync(s => s.Id == request.SubjectId, ct);
        if (subject is null) throw new InvalidOperationException("Subject not found");

        var classroom = await db.Classrooms.FirstOrDefaultAsync(c => c.Id == request.PreferredClassroomId, ct);
        if (classroom is null) throw new InvalidOperationException("Classroom not found");

        entity.Name = request.Name.Trim();
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.Subject = subject;
        entity.PreferredClassroom = classroom;

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Handle(DeleteCourse request, CancellationToken ct)
    {
        var entity = await db.Courses.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity is null) return false;

        db.Courses.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<CourseResponse>> Handle(GetCourses request, CancellationToken ct)
    {
        var entities = await db.Courses
            .AsNoTracking()
            .Include(c => c.Subject)
            .Include(c => c.PreferredClassroom)
            .ToListAsync(ct);

        return entities.Select(CourseResponse.FromEntity);
    }

    public async Task<CourseResponse?> Handle(GetCourseById request, CancellationToken ct)
    {
        var entity = await db.Courses
            .AsNoTracking()
            .Include(c => c.Subject)
            .Include(c => c.PreferredClassroom)
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        return entity is null ? null : CourseResponse.FromEntity(entity);
    }


}
