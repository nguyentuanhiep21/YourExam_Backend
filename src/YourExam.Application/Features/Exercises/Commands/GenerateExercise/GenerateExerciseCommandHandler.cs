using MediatR;
using Microsoft.EntityFrameworkCore;
using YourExam.Application.Interfaces;
using YourExam.Application.Services.QuestionGeneration;
using YourExam.Domain.Entities;

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

        if (request.Subject.ToLower() == "tiengviet" && request.Format == Domain.Enums.QuestionFormat.Essay)
        {
            if (request.ExerciseType == Domain.Enums.ExerciseType.OddOneOut || 
                request.ExerciseType == Domain.Enums.ExerciseType.FillInBlank)
            {
                return new GenerateExerciseResponse 
                { 
                    Success = false, 
                    ErrorMessage = $"Dạng bài tập này không hỗ trợ định dạng Tự luận. Vui lòng chọn định dạng Trắc nghiệm."
                };
            }
        }

        // ── Luồng Động: Tiếng Việt bốc trực tiếp từ Dictionaries (không dùng DB) ──
        if (request.Subject.ToLower() == "tiengviet")
        {
            var strategy = _questionGeneratorFactory.GetStrategy(request.Subject, request.GradeLevel, request.ExerciseType);

            // Dummy template chỉ mang ExerciseType và Topic, không cần VariablesConfig hay DB
            var dummyTemplate = new QuestionTemplate
            {
                Subject = request.Subject,
                GradeLevel = request.GradeLevel,
                ExerciseType = request.ExerciseType,
                Topic = request.Topic ?? string.Empty,
                Difficulty = request.Difficulty
            };

            for (int i = 0; i < request.Quantity; i++)
            {
                try
                {
                    var exercise = await strategy.GenerateAsync(dummyTemplate, request.Format);
                    
                    if (request.Format == Domain.Enums.QuestionFormat.Essay)
                    {
                        exercise.Choices.Clear();
                    }

                    response.Data.Add(exercise);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DEBUG] LiteratureGrade1Strategy error: {ex.Message}");
                }
            }

            return response;
        }

        // ── Luồng Tĩnh: Toán và các môn còn lại → đọc từ DB ─────────────────
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
        var dbStrategy = _questionGeneratorFactory.GetStrategy(request.Subject, request.GradeLevel, request.ExerciseType);

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
                var generatedExercise = await dbStrategy.GenerateAsync(template, request.Format);
                
                if (request.Format == Domain.Enums.QuestionFormat.Essay)
                {
                    generatedExercise.Choices.Clear();
                }

                response.Data.Add(generatedExercise);
                generatedCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Error generating exercise from template {template.Id}: {ex}");
            }
        }

        return response;
    }
}
