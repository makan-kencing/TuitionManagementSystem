

namespace TuitionManagementSystem.Web.Features.Subject;

using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuitionManagementSystem.Web.ViewModels.Subject;


[Route("subjects")]
public class SubjectController(IMediator mediator, ApplicationDbContext db) : Controller
{
    private readonly ApplicationDbContext _db = db;
    // GET ALL SUBJECTS
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var subjects = await mediator.Send(new GetSubjects());
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
        // check for archived duplicate
        var archived = await db.Subjects
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == subject.Name && s.DeletedAt != null);

        if (archived != null)
        {
            return View("SubjectArchive", archived);
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
    public async Task<IActionResult> Archive(int id)
    {
        var ok = await mediator.Send(new ArchiveSubject(id));
        if (!ok) return NotFound();
        return RedirectToAction(nameof(Index));
    }

    // RESTORE (soft delete)
    [HttpPost("{id:int}/restore")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id)
    {
        var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.Id == id);
        if (subject is null) return NotFound();
        subject.DeletedAt = null;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(GetSubject), new { id });
    }


}
