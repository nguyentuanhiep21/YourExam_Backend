using MediatR;
using YourExam.Application.DTOs.QuestionTemplates;
using YourExam.Domain.Interfaces;

namespace YourExam.Application.Features.QuestionTemplates.Queries.GetAllQuestionTemplates;

public class GetAllQuestionTemplatesQueryHandler : IRequestHandler<GetAllQuestionTemplatesQuery, List<QuestionTemplateDto>>
{
    private readonly IQuestionTemplateRepository _repository;

    public GetAllQuestionTemplatesQueryHandler(IQuestionTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<QuestionTemplateDto>> Handle(GetAllQuestionTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _repository.GetActiveByCriteriaAsync(
            subject: request.Subject,
            difficulty: request.Difficulty,
            gradeLevel: request.GradeLevel,
            exerciseType: request.ExerciseType,
            topic: null, // Topic is not in GetAllQuestionTemplatesQuery currently
            offset: request.Offset,
            limit: request.Quantity,
            cancellationToken: cancellationToken
        );

        return templates.Select(q => new QuestionTemplateDto(
            q.Id,
            q.Subject,
            q.GradeLevel,
            q.Topic,
            q.Difficulty,
            q.ExerciseType,
            q.ContentTemplate,
            q.VariablesConfig,
            q.AnswerFormula,
            q.DistractorLogic,
            q.EqualTargetVariable,
            q.MasterTemplateId,
            q.IsActive
        )).ToList();
    }
}

