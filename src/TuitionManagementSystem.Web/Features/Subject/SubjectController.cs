
namespace TuitionManagementSystem.Web.Features.Subject;
using MediatR;
using Microsoft.AspNetCore.Mvc;


[Route("subjects")]
public class SubjectController : Controller
{
    private readonly IMediator _mediator;
    public SubjectController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetAll()
        => Ok(await _mediator.Send(new GetSubjects()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubjectResponse>> GetById(int id)
    {
        var result = await _mediator.Send(new GetSubjectById(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SubjectResponse>> Create([FromBody] CreateSubject command)
    {
        var created = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSubject command)
    {
        if (id != command.Id) return BadRequest();
        var updated = await _mediator.Send(command);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediator.Send(new DeleteSubject(id));
        return deleted ? NoContent() : NotFound();
    }
}
