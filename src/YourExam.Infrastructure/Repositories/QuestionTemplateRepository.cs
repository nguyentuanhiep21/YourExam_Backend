using Microsoft.EntityFrameworkCore;
using YourExam.Domain.Entities;
using YourExam.Domain.Enums;
using YourExam.Domain.Interfaces;
using YourExam.Infrastructure.Data;

namespace YourExam.Infrastructure.Repositories;

public class QuestionTemplateRepository : IQuestionTemplateRepository
{
    private readonly ApplicationDbContext _context;

    public QuestionTemplateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<QuestionTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.QuestionTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public Task<List<QuestionTemplate>> GetActiveByCriteriaAsync(
        string? subject, 
        int? difficulty, 
        int? gradeLevel, 
        ExerciseType? exerciseType, 
        string? topic, 
        int? offset = 0,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.QuestionTemplates.AsNoTracking().Where(q => q.IsActive);

        if (!string.IsNullOrEmpty(subject))
        {
            query = query.Where(q => q.Subject.ToLower() == subject.ToLower());
        }

        if (difficulty.HasValue)
        {
            query = query.Where(q => q.Difficulty == difficulty.Value);
        }

        if (gradeLevel.HasValue)
        {
            query = query.Where(q => q.GradeLevel == gradeLevel.Value);
        }

        if (exerciseType.HasValue)
        {
            query = query.Where(q => q.ExerciseType == exerciseType.Value);
        }
        
        if (!string.IsNullOrEmpty(topic))
        {
            query = query.Where(q => q.Topic.ToLower() == topic.ToLower());
        }

        if (offset.HasValue && offset.Value > 0)
        {
            query = query.Skip(offset.Value);
        }

        if (limit.HasValue && limit.Value > 0)
        {
            query = query.Take(limit.Value);
        }

        return query.ToListAsync(cancellationToken);
    }

    public Task AddRangeAsync(IEnumerable<QuestionTemplate> templates, CancellationToken cancellationToken = default)
    {
        _context.QuestionTemplates.AddRange(templates);
        return Task.CompletedTask;
    }
}
