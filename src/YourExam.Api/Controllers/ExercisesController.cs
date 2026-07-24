using MediatR;
using Microsoft.AspNetCore.Mvc;
using YourExam.Application.Features.Exercises.Commands.GenerateExercise;

namespace YourExam.Api.Controllers;

[ApiController]
[Route("api/exercises")]
public class ExercisesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExercisesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateExerciseCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
