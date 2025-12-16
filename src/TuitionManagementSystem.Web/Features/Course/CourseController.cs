namespace TuitionManagementSystem.Web.Features.Course;

using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Course;
using Microsoft.AspNetCore.Mvc.Rendering;
using Subject;
using Classroom;

[Route("courses")]
public class CourseController(IMediator mediator) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
        => View("CourseIndex", await mediator.Send(new GetCourses()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var item = await mediator.Send(new GetCourseById(id));
        if (item is null) return NotFound();
        return View("CourseDetails", item);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        await LoadLookups();
        return View("CourseCreate", new CourseFormVm());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseFormVm vm)
    {
        if (!ModelState.IsValid)
        {
            await LoadLookups();
            return View("CourseCreate", vm);
        }


    var created = await mediator.Send(new CreateCourse(
            vm.Name,
            vm.Description,
            vm.Price,
            vm.SubjectId,
            vm.PreferredClassroomId));

        return RedirectToAction(nameof(Details), new { id = created.Id });
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await mediator.Send(new GetCourseById(id));
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
        await LoadLookups();
        return View("CourseEdit", vm);
    }

    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseFormVm vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            await  LoadLookups();
            return View("CourseEdit", vm);
        }

        var ok = await mediator.Send(new UpdateCourse(
            vm.Id,
            vm.Name,
            vm.Description,
            vm.Price,
            vm.SubjectId,
            vm.PreferredClassroomId));

        if (!ok) return NotFound();
        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await mediator.Send(new DeleteCourse(id));
        if (!ok) return NotFound();
        return RedirectToAction(nameof(Index));
    }
    private async Task LoadLookups()
    {
        ViewBag.Subjects = (await mediator.Send(new GetSubjects()))
            .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name });
        ViewBag.Classrooms = (await mediator.Send(new GetClassrooms()))
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Location });
    }

}

