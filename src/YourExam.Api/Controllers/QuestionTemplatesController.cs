using MediatR;
using Microsoft.AspNetCore.Mvc;
using YourExam.Application.DTOs;
using YourExam.Application.Features.QuestionTemplates.Queries.GetAllQuestionTemplates;
using YourExam.Application.Features.QuestionTemplates.Queries.GetQuestionTemplateById;
using YourExam.Application.Features.QuestionTemplates.Commands.AutoGenerateTemplates;

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
    public async Task<ActionResult<List<QuestionTemplateDto>>> GetAll([FromQuery] string? subject, [FromQuery] int? difficulty, [FromQuery] int? gradeLevel, [FromQuery] Domain.Enums.ExerciseType? exerciseType, [FromQuery] int? quantity)
    {
        var query = new GetAllQuestionTemplatesQuery 
        { 
            Subject = subject, 
            Difficulty = difficulty,
            GradeLevel = gradeLevel,
            ExerciseType = exerciseType,
            Quantity = quantity
        };
        var templates = await _mediator.Send(query);
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionTemplateDto>> GetById(int id)
    {
        var template = await _mediator.Send(new GetQuestionTemplateByIdQuery(id));
        return template != null ? Ok(template) : NotFound();
    }

    [HttpPost("auto-generate")]
    public async Task<ActionResult<int>> AutoGenerate([FromBody] AutoGenerateTemplatesCommand command)
    {
        var count = await _mediator.Send(command);
        return Ok(new { Message = $"Đã tạo thành công {count} templates", Count = count });
    }
}
