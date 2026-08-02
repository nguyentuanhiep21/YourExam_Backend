using MediatR;
using Microsoft.EntityFrameworkCore;
using YourExam.Application.DTOs;
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

        if (request.QuestionType.HasValue)
        {
            query = query.Where(q => q.QuestionType == request.QuestionType.Value);
        }

        if (request.Quantity.HasValue && request.Quantity.Value > 0)
        {
            query = query.Take(request.Quantity.Value);
        }

        var templates = await query.ToListAsync(cancellationToken);

        return templates.Select(q => new QuestionTemplateDto
        {
            Id = q.Id,
            Subject = q.Subject,
            GradeLevel = q.GradeLevel,
            Topic = q.Topic,
            Difficulty = q.Difficulty,
            QuestionType = q.QuestionType,
            ContentTemplate = q.ContentTemplate,
            VariablesConfig = q.VariablesConfig,
            AnswerFormula = q.AnswerFormula,
            DistractorLogic = q.DistractorLogic,
            EqualTargetVariable = q.EqualTargetVariable,
            IsActive = q.IsActive
        }).ToList();
    }
}
