using YourExam.Domain.Entities;
using YourExam.Domain.Enums;

namespace YourExam.Domain.Interfaces;

public interface IQuestionTemplateRepository
{
    Task<QuestionTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    Task<List<QuestionTemplate>> GetActiveByCriteriaAsync(
        string? subject, 
        int? difficulty, 
        int? gradeLevel, 
        ExerciseType? exerciseType, 
        string? topic, 
        int? offset = 0,
        int? limit = null,
        CancellationToken cancellationToken = default);
        
    Task AddRangeAsync(IEnumerable<QuestionTemplate> templates, CancellationToken cancellationToken = default);
}
