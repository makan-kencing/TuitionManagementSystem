namespace TuitionManagementSystem.Web.Features.Classroom;

using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("classrooms")]
public class ClassroomController(IMediator mediator) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
        => View("ClassroomIndex", await mediator.Send(new GetClassrooms()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var item = await mediator.Send(new GetClassroomById(id));
        if (item is null) return NotFound();
        return View("ClassroomDetails", item);
    }

    [HttpGet("create")]
    public IActionResult Create() => View("ClassroomCreate");

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClassroomFormVM vm)
    {
        if (!ModelState.IsValid) return View("ClassroomCreate", vm);
        var created = await mediator.Send(new CreateClassroom(vm.Location, vm.MaxCapacity));
        return RedirectToAction(nameof(Details), new { id = created.Id });
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await mediator.Send(new GetClassroomById(id));
        if (item is null) return NotFound();
        var vm = new ClassroomFormVM { Id = item.Id, Location = item.Location, MaxCapacity = item.MaxCapacity };
        return View("ClassroomEdit", vm);
    }

    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClassroomFormVM vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View("ClassroomEdit", vm);
        var ok = await mediator.Send(new UpdateClassroom(vm.Id, vm.Location, vm.MaxCapacity));
        if (!ok) return NotFound();
        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await mediator.Send(new DeleteClassroom(id));
        if (!ok) return NotFound();
        return RedirectToAction(nameof(Index));
    }
}
