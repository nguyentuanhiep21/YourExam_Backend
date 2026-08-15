using MediatR;
using Microsoft.EntityFrameworkCore;
using YourExam.Application.DTOs.QuestionTemplates;
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

