using MediatR;
using YourExam.Application.DTOs;

namespace YourExam.Application.Features.QuestionTemplates.Queries.GetAllQuestionTemplates;

public class GetAllQuestionTemplatesQuery : IRequest<List<QuestionTemplateDto>>
{
    // Optionally add pagination or filtering here later
    public string? Subject { get; set; }
    public int? Difficulty { get; set; }
    public int? GradeLevel { get; set; }
    public int? QuestionType { get; set; }
    public int? Quantity { get; set; }
}
