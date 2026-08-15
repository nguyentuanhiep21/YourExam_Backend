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

    [HttpPost("export/docx")]
    public async Task<IActionResult> ExportToDocx([FromBody] YourExam.Application.Features.Exercises.Commands.ExportExercisesToDocx.ExportExercisesToDocxCommand command)
    {
        var result = await _mediator.Send(command);
        return File(result.ZipBytes, "application/zip", result.ZipFileName);
    }
}
