using MediatR;
using YourExam.Application.DTOs.QuestionTemplates;
using YourExam.Domain.Interfaces;

namespace YourExam.Application.Features.QuestionTemplates.Queries.GetQuestionTemplateById;

public class GetQuestionTemplateByIdQueryHandler : IRequestHandler<GetQuestionTemplateByIdQuery, QuestionTemplateDto?>
{
    private readonly IQuestionTemplateRepository _repository;

    public GetQuestionTemplateByIdQueryHandler(IQuestionTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<QuestionTemplateDto?> Handle(GetQuestionTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var template = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (template == null) return null;

        return new QuestionTemplateDto(
            template.Id,
            template.Subject,
            template.GradeLevel,
            template.Topic,
            template.Difficulty,
            template.ExerciseType,
            template.ContentTemplate,
            template.VariablesConfig,
            template.AnswerFormula,
            template.DistractorLogic,
            template.EqualTargetVariable,
            template.MasterTemplateId,
            template.IsActive
        );
    }
}

