using MediatR;
using Microsoft.EntityFrameworkCore;
using YourExam.Application.DTOs.QuestionTemplates;
using YourExam.Application.Interfaces;

namespace YourExam.Application.Features.QuestionTemplates.Queries.GetAllQuestionTemplates;

public class GetAllQuestionTemplatesQueryHandler : IRequestHandler<GetAllQuestionTemplatesQuery, List<QuestionTemplateDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllQuestionTemplatesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<QuestionTemplateDto>> Handle(GetAllQuestionTemplatesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.QuestionTemplates.AsNoTracking().Where(q => q.IsActive);

        if (!string.IsNullOrEmpty(request.Subject))
        {
            query = query.Where(q => q.Subject == request.Subject);
        }

        if (request.Difficulty.HasValue)
        {
            query = query.Where(q => q.Difficulty == request.Difficulty.Value);
        }

        if (request.GradeLevel.HasValue)
        {
            query = query.Where(q => q.GradeLevel == request.GradeLevel.Value);
        }

        if (request.ExerciseType.HasValue)
        {
            query = query.Where(q => q.ExerciseType == request.ExerciseType.Value);
        }

        if (request.Quantity.HasValue && request.Quantity.Value > 0)
        {
            query = query.Take(request.Quantity.Value);
        }

        var templates = await query.ToListAsync(cancellationToken);

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

