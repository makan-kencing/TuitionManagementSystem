namespace TuitionManagementSystem.Web.Features.Course;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using Models.User;
using CourseTeacherEntity = TuitionManagementSystem.Web.Models.Class.CourseTeacher;

public class CourseRequestHandler(ApplicationDbContext db) :
    IRequestHandler<CreateCourse, CourseResponse>,
    IRequestHandler<UpdateCourse, bool>,
    IRequestHandler<DeleteCourse, bool>,
    IRequestHandler<GetCourses, IEnumerable<CourseResponse>>,
    IRequestHandler<GetCourseById, CourseResponse?>,
    IRequestHandler<GetCourseIndexRows, IReadOnlyList<CourseIndexRowResponse>>,
    IRequestHandler<GetCourseIndexRowsFiltered, IReadOnlyList<CourseIndexRowResponse>>,
    IRequestHandler<UpdateCourseLookupsInline, bool>,
    IRequestHandler<UpdateCourseTeacherInline, bool>,
    IRequestHandler<GetTeacherLookups, IReadOnlyList<TeacherLookupResponse>>


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

    public async Task<IReadOnlyList<CourseIndexRowResponse>> Handle(GetCourseIndexRows request, CancellationToken ct)
    {
        var courses = await db.Courses
            .AsNoTracking()
            .Include(c => c.Subject)
            .Include(c => c.PreferredClassroom)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Price,
                SubjectId = c.Subject.Id,
                SubjectName = c.Subject.Name,
                PreferredClassroomId = c.PreferredClassroom.Id,
                PreferredClassroomLocation = c.PreferredClassroom.Location
            })
            .ToListAsync(ct);

        var courseIds = courses.Select(c => c.Id).ToList();

        var teacherRows = await (
            from ctRow in db.CourseTeachers.AsNoTracking()
            join teacher in db.Users.OfType<Teacher>().AsNoTracking() on ctRow.TeacherId equals teacher.Id
            join acc in db.Accounts.AsNoTracking() on teacher.AccountId equals acc.Id
            where courseIds.Contains(ctRow.CourseId)
                  && acc.DeletedAt == null
            select new
            {
                ctRow.CourseId,
                TeacherId = teacher.Id,
                TeacherName = (acc.DisplayName ?? acc.Username)
            }
        ).ToListAsync(ct);

        var teachersByCourse = teacherRows
            .GroupBy(x => x.CourseId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(x => x.TeacherName).ToList()
            );

        return courses.Select(c =>
        {
            teachersByCourse.TryGetValue(c.Id, out var tlist);
            string teacherNames;
            int? teacherId;

            if (tlist == null || tlist.Count == 0)
            {
                teacherNames = "-";
                teacherId = null;
            }
            else
            {
                teacherNames = string.Join(", ", tlist.Select(x => x.TeacherName));
                teacherId = tlist[0].TeacherId;
            }

            return new CourseIndexRowResponse(
                c.Id,
                c.Name,
                c.Price,
                c.SubjectId,
                c.SubjectName,
                c.PreferredClassroomId,
                c.PreferredClassroomLocation,
                teacherNames,
                teacherId
            );
        }).ToList();
    }

    public async Task<IReadOnlyList<CourseIndexRowResponse>> Handle(GetCourseIndexRowsFiltered request, CancellationToken ct)
    {
        var query = db.Courses
            .AsNoTracking()
            .Include(c => c.Subject)
            .Include(c => c.PreferredClassroom)
            .AsQueryable();

        if (request.SubjectId.HasValue)
            query = query.Where(c => c.Subject.Id == request.SubjectId.Value);
        if (request.ClassroomId.HasValue)
            query = query.Where(c => c.PreferredClassroom.Id == request.ClassroomId.Value);

        var courses = await query
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Price,
                SubjectId = c.Subject.Id,
                SubjectName = c.Subject.Name,
                PreferredClassroomId = c.PreferredClassroom.Id,
                PreferredClassroomLocation = c.PreferredClassroom.Location
            })
            .ToListAsync(ct);

        var courseIds = courses.Select(c => c.Id).ToList();

        var teacherRows = await (
            from ctRow in db.CourseTeachers.AsNoTracking()
            join teacher in db.Users.OfType<Teacher>().AsNoTracking() on ctRow.TeacherId equals teacher.Id
            join acc in db.Accounts.AsNoTracking() on teacher.AccountId equals acc.Id
            where courseIds.Contains(ctRow.CourseId)
                  && acc.DeletedAt == null
            select new
            {
                ctRow.CourseId,
                TeacherId = teacher.Id,
                TeacherName = (acc.DisplayName ?? acc.Username)
            }
        ).ToListAsync(ct);

        var teachersByCourse = teacherRows
            .GroupBy(x => x.CourseId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(x => x.TeacherName).ToList()
            );

        var result = courses.Select(c =>
        {
            teachersByCourse.TryGetValue(c.Id, out var tlist);

            string teacherNames;
            int? teacherId;

            if (tlist == null || tlist.Count == 0)
            {
                teacherNames = "-";
                teacherId = null;
            }
            else
            {
                teacherNames = string.Join(", ", tlist.Select(x => x.TeacherName));
                teacherId = tlist[0].TeacherId;
            }

            return new CourseIndexRowResponse(
                c.Id,
                c.Name,
                c.Price,
                c.SubjectId,
                c.SubjectName,
                c.PreferredClassroomId,
                c.PreferredClassroomLocation,
                teacherNames,
                teacherId
            );
        }).ToList();

        if (request.TeacherId.HasValue)
        {
            result = result.Where(r => r.TeacherId == request.TeacherId.Value).ToList();
        }

        return result;
    }

    public async Task<bool> Handle(UpdateCourseLookupsInline request, CancellationToken ct)
    {
        var course = await db.Courses
            .Include(c => c.Subject)
            .Include(c => c.PreferredClassroom)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, ct);

        if (course is null) return false;

        var subject = await db.Subjects.FirstOrDefaultAsync(s => s.Id == request.SubjectId, ct);
        if (subject is null) return false;

        var classroom = await db.Classrooms.FirstOrDefaultAsync(r => r.Id == request.PreferredClassroomId, ct);
        if (classroom is null) return false;

        course.Subject = subject;
        course.PreferredClassroom = classroom;

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> Handle(UpdateCourseTeacherInline request, CancellationToken ct)
    {
        var courseExists = await db.Courses.AnyAsync(c => c.Id == request.CourseId, ct);
        if (!courseExists) return false;

        var existing = await db.CourseTeachers
            .Where(x => x.CourseId == request.CourseId)
            .ToListAsync(ct);

        if (existing.Count > 0)
            db.CourseTeachers.RemoveRange(existing);

        if (request.TeacherId is null)
        {
            await db.SaveChangesAsync(ct);
            return true;
        }

        var teacherExists = await (
            from t in db.Users.OfType<Teacher>()
            join a in db.Accounts on t.AccountId equals a.Id
            where t.Id == request.TeacherId.Value && a.DeletedAt == null
            select t.Id
        ).AnyAsync(ct);

        if (!teacherExists) return false;

        db.CourseTeachers.Add(new CourseTeacherEntity
        {
            CourseId = request.CourseId,
            TeacherId = request.TeacherId.Value
        });

        await db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<IReadOnlyList<TeacherLookupResponse>> Handle(GetTeacherLookups request, CancellationToken ct)
    {
        return await db.Users
            .OfType<Teacher>()
            .AsNoTracking()
            .Include(t => t.Account)
            .Where(t => t.Account.DeletedAt == null)
            .OrderBy(t => t.Account.DisplayName ?? t.Account.Username)
            .Select(t => new TeacherLookupResponse(
                t.Id,
                t.Account.DisplayName ?? t.Account.Username
            ))
            .ToListAsync(ct);
    }


}

