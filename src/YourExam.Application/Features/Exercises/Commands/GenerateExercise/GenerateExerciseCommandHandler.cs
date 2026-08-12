using MediatR;
using Microsoft.EntityFrameworkCore;
using YourExam.Application.Interfaces;
using YourExam.Application.Services.QuestionGeneration;

namespace YourExam.Application.Features.Exercises.Commands.GenerateExercise;

public class GenerateExerciseCommandHandler : IRequestHandler<GenerateExerciseCommand, GenerateExerciseResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IQuestionGeneratorFactory _questionGeneratorFactory;

    public GenerateExerciseCommandHandler(
        IApplicationDbContext dbContext,
        IQuestionGeneratorFactory questionGeneratorFactory)
    {
        _dbContext = dbContext;
        _questionGeneratorFactory = questionGeneratorFactory;
    }

    public async Task<GenerateExerciseResponse> Handle(GenerateExerciseCommand request, CancellationToken cancellationToken)
    {
        var response = new GenerateExerciseResponse { Success = true };

        // 1. Fetch matching templates from DB. 
        var query = _dbContext.QuestionTemplates
            .Where(t => t.IsActive 
                        && t.Subject.ToLower() == request.Subject.ToLower()
                        && t.GradeLevel == request.GradeLevel
                        && t.Difficulty == request.Difficulty
                        && t.ExerciseType == request.ExerciseType);

        if (!string.IsNullOrEmpty(request.Topic))
        {
            query = query.Where(t => t.Topic.ToLower() == request.Topic.ToLower());
        }

        var matchingTemplates = await query.ToListAsync(cancellationToken);

        if (!matchingTemplates.Any())
        {
            return response;
        }

        // Lấy strategy từ Factory dựa trên request
        var strategy = _questionGeneratorFactory.GetStrategy(request.Subject, request.GradeLevel, request.ExerciseType);

        // Trộn mảng ngẫu nhiên (Shuffle in-memory)
        var random = new Random();
        matchingTemplates = matchingTemplates.OrderBy(x => random.Next()).ToList();

        int generatedCount = 0;
        int templateIndex = 0;

        while (generatedCount < request.Quantity && templateIndex < matchingTemplates.Count)
        {
            var template = matchingTemplates[templateIndex];
            templateIndex++;
            
            try
            {
                var generatedExercise = await strategy.GenerateAsync(template);
                response.Data.Add(generatedExercise);
                generatedCount++;
            }
            catch (Exception ex)
            {
                // Nếu strategy throw lỗi (ví dụ không thể generate sau N lần thử), bỏ qua template này và thử template khác
                Console.WriteLine($"[DEBUG] Error generating exercise from template {template.Id}: {ex}");
            }
        }

        return response;
    }
}
