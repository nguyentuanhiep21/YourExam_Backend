using MediatR;
using Microsoft.EntityFrameworkCore;
using YourExam.Application.DTOs;
using YourExam.Application.Interfaces;

namespace YourExam.Application.Features.QuestionTemplates.Queries.GetQuestionTemplateById;

public class GetQuestionTemplateByIdQueryHandler : IRequestHandler<GetQuestionTemplateByIdQuery, QuestionTemplateDto?>
{
    private readonly IApplicationDbContext _context;

    public GetQuestionTemplateByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<QuestionTemplateDto?> Handle(GetQuestionTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var template = await _context.QuestionTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

        if (template == null) return null;

        return new QuestionTemplateDto
        {
            Id = template.Id,
            Subject = template.Subject,
            Topic = template.Topic,
            Difficulty = template.Difficulty,
            QuestionType = template.QuestionType,
            ContentTemplate = template.ContentTemplate,
            VariablesConfig = template.VariablesConfig,
            AnswerFormula = template.AnswerFormula,
            DistractorLogic = template.DistractorLogic,
            IsActive = template.IsActive
        };
    }
}
