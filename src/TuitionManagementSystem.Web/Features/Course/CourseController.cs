namespace TuitionManagementSystem.Web.Features.Course;

using System.Linq;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Class;
using TuitionManagementSystem.Web.Features.Classroom;
using TuitionManagementSystem.Web.Features.Subject;
using TuitionManagementSystem.Web.ViewModels.Course;

[Route("courses")]
[Authorize (Roles = "Administrator")]
public class CourseController(IMediator mediator, ApplicationDbContext db) : Controller
{
    private readonly ApplicationDbContext _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewBag.Subjects = (await mediator.Send(new GetSubjects(), ct))
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
            .ToList();

        ViewBag.Classrooms = (await mediator.Send(new GetClassrooms(), ct))
            .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Location })
            .ToList();

        ViewBag.Teachers = (await mediator.Send(new GetTeacherLookups(), ct))
            .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.FullName })
            .ToList();

        var rows = await mediator.Send(new GetCourseIndexRows(), ct);
        return View("CourseIndex", rows);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var item = await mediator.Send(new GetCourseById(id), ct);
        if (item is null) return NotFound();

        return View("CourseDetails", item);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await LoadLookups(ct);
        return View("CourseCreate", new CourseFormVm());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseFormVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookups(ct);
            return View("CourseCreate", vm);
        }

        var created = await mediator.Send(new CreateCourse(
            vm.Name,
            vm.Description,
            vm.Price,
            vm.SubjectId,
            vm.PreferredClassroomId), ct);


        return RedirectToAction("Details", new { id = created.Id });


    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var item = await mediator.Send(new GetCourseById(id), ct);
        if (item is null) return NotFound();

        var vm = new CourseFormVm
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            SubjectId = item.SubjectId,
            PreferredClassroomId = item.PreferredClassroomId
        };

        await LoadLookups(ct);
        return View("CourseEdit", vm);
    }

    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseFormVm vm, CancellationToken ct)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await LoadLookups(ct);
            return View("CourseEdit", vm);
        }

        var ok = await mediator.Send(new UpdateCourse(
            vm.Id,
            vm.Name,
            vm.Description,
            vm.Price,
            vm.SubjectId,
            vm.PreferredClassroomId), ct);

        if (!ok) return NotFound();

        return RedirectToAction("Details", new { id = vm.Id });

    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await mediator.Send(new DeleteCourse(id), ct);
        if (!ok) return NotFound();
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadLookups(CancellationToken ct)
    {
        ViewBag.Subjects = (await mediator.Send(new GetSubjects(), ct))
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
            .ToList();

        ViewBag.Classrooms = (await mediator.Send(new GetClassrooms(), ct))
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Location })
            .ToList();
    }

    public sealed class CourseInlineUpdateDto
    {
        public int SubjectId { get; set; }
        public int PreferredClassroomId { get; set; }
    }

    public sealed class CourseTeacherInlineDto
    {
        public int? TeacherId { get; set; }
    }

    [HttpPost("{id:int}/inline-lookups")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InlineLookups(int id, [FromBody] CourseInlineUpdateDto dto, CancellationToken ct)
    {
        var ok = await mediator.Send(new UpdateCourseLookupsInline(id, dto.SubjectId, dto.PreferredClassroomId), ct);
        if (!ok) return BadRequest(new { ok = false });
        return Json(new { ok = true });
    }

    [HttpPost("{id:int}/inline-teacher")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InlineTeacher(int id, [FromBody] CourseTeacherInlineDto dto, CancellationToken ct)
    {
        var ok = await mediator.Send(new UpdateCourseTeacherInline(id, dto.TeacherId), ct);
        if (!ok) return BadRequest(new { ok = false });
        return Json(new { ok = true });
    }

    // Optional: keep as API, but fix route + parameter usage
    [HttpGet("api/by-subject/{subjectId:int}")]
    public async Task<IActionResult> GetCoursesBySubject(int subjectId, CancellationToken ct)
    {
        var courses = await mediator.Send(new GetCourses(), ct);
        return Ok(courses
            .Where(c => c.SubjectId == subjectId)
            .Select(c => new { c.Id, c.Name, c.Description, c.Price }));
    }
}
