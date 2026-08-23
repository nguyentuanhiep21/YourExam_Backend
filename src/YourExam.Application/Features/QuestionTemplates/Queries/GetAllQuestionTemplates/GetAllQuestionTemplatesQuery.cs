using MediatR;
using YourExam.Application.DTOs.QuestionTemplates;

namespace YourExam.Application.Features.QuestionTemplates.Queries.GetAllQuestionTemplates;

public class GetAllQuestionTemplatesQuery : IRequest<List<QuestionTemplateDto>>
{
    // Optionally add pagination or filtering here later
    public string? Subject { get; set; }
    public int? Difficulty { get; set; }
    public int? GradeLevel { get; set; }
    public Domain.Enums.ExerciseType? ExerciseType { get; set; }
    public int? Quantity { get; set; } // acts as Limit
    public int? Offset { get; set; }
}

