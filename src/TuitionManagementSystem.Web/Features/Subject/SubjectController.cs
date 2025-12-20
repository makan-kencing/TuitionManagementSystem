

namespace TuitionManagementSystem.Web.Features.Subject;

using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using TuitionManagementSystem.Web.ViewModels.Subject;


[Route("subjects")]
[Authorize(Roles = "Administrator")]
public class SubjectController(IMediator mediator, ApplicationDbContext db) : Controller
{
    private readonly ApplicationDbContext _db = db;
    // GET ALL SUBJECTS
    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] bool showArchivedOnly = false)
    {
        ViewData["ShowArchivedOnly"] = showArchivedOnly;
        var subjects = await mediator.Send(new GetSubjects(IncludeArchived: true)); // get all, filter in view
        return View("SubjectIndex", subjects);
    }

    //GET SUBJECT BY ID
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSubject(int id)
    {
        var subject = await mediator.Send(new GetSubjectById(id));
        if (subject is null) return NotFound();
        return this.View("SubjectDetails", subject);
    }

    //GET SUBJECT CREATE FOR DISPLAY
    [HttpGet("create")]
    public IActionResult Create() => View("SubjectCreate", new SubjectFormVm());


    //POST SUBJECT CREATE
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubjectFormVm subject)
    {
        subject.Name = subject.Name?.Trim() ?? string.Empty;

        var existing = await db.Subjects
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == subject.Name);

        if (existing is not null && existing.DeletedAt != null)
        {
            return View("SubjectRestore", existing);
        }

        if (existing is not null)
        {
            ModelState.AddModelError(nameof(subject.Name), "A subject with this name already exists.");
        }

        if (!ModelState.IsValid) return View("SubjectCreate", subject);

        var created = await mediator.Send(new CreateSubject(subject.Name, subject.Description));
        return RedirectToAction(nameof(GetSubject), new{id = created.Id});
    }



    //GET SUBJECT EDIT
    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var subject = await mediator.Send(new GetSubjectById(id));
        if(subject == null) return NotFound();

        var subjectEdit = new SubjectFormVm { Id = subject.Id, Name = subject.Name, Description = subject.Description };
        return View("SubjectEdit", subjectEdit);
    }


    //POST SUBJECT EDIT
    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubjectFormVm subject)
    {
        if (id != subject.Id) return this.BadRequest();
        if (!ModelState.IsValid) return this.View("SubjectEdit", subject);

        var ok = await mediator.Send(new UpdateSubject(subject.Id, subject.Name, subject.Description));
        if(!ok) return NotFound();
        return RedirectToAction(nameof(GetSubject), new{id = subject.Id});
    }

    // ARCHIVE (soft delete)
    [HttpPost("{id:int}/archive")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(int id, [FromQuery] bool showArchivedOnly = false)
    {
        var ok = await mediator.Send(new ArchiveSubject(id));
        if (!ok) return NotFound();
        return RedirectToAction(nameof(Index), new { showArchivedOnly });
    }

    // RESTORE (soft delete)
    [HttpPost("{id:int}/restore")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id, [FromQuery] bool showArchivedOnly = false)
    {
        var ok = await mediator.Send(new RestoreSubject(id));
        if (!ok) return NotFound();
        return RedirectToAction(nameof(Index), new { showArchivedOnly });
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, [FromQuery] bool showArchivedOnly = false)
    {
        var entity = await _db.Subjects
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (entity is null) return NotFound();

        _db.Subjects.Remove(entity);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { showArchivedOnly });
    }

    [HttpGet("api")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSubjectsApi([FromQuery] bool withStats = false)
    {
        var subjects = await mediator.Send(new GetSubjects());

        if (!withStats)
        {
            return Ok(subjects.Select(s => new {
                s.Id,
                s.Name,
                s.Description
            }));
        }

        var subjectsWithStats = new List<object>();
        foreach (var subject in subjects)
        {
            var courseCount = await _db.Courses
                .CountAsync(c => c.SubjectId == subject.Id);

            var studentCount = await _db.Enrollments
                .Include(e => e.Course)
                .Where(e => e.Course.SubjectId == subject.Id && e.Status == Enrollment.EnrollmentStatus.Active)
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync();

            subjectsWithStats.Add(new
            {
                subject.Id,
                subject.Name,
                subject.Description,
                CourseCount = courseCount,
                StudentCount = studentCount
            });
        }

        subjectsWithStats = subjectsWithStats.OrderByDescending(s => ((dynamic)s).CourseCount).ToList();

        return Ok(subjectsWithStats);
    }
}
