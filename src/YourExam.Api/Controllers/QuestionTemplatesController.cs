using MediatR;
using Microsoft.AspNetCore.Mvc;
using YourExam.Application.DTOs;
using YourExam.Application.Features.QuestionTemplates.Queries.GetAllQuestionTemplates;
using YourExam.Application.Features.QuestionTemplates.Queries.GetQuestionTemplateById;

namespace YourExam.Api.Controllers;

[ApiController]
[Route("api/question-templates")]
public class QuestionTemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuestionTemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuestionTemplateDto>>> GetAll([FromQuery] string? subject, [FromQuery] int? difficulty)
    {
        var query = new GetAllQuestionTemplatesQuery { Subject = subject, Difficulty = difficulty };
        var templates = await _mediator.Send(query);
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionTemplateDto>> GetById(int id)
    {
        var template = await _mediator.Send(new GetQuestionTemplateByIdQuery(id));
        if (template == null)
        {
            return NotFound();
        }
        return Ok(template);
    }
}
