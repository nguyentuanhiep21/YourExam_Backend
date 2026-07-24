using MediatR;
using Microsoft.EntityFrameworkCore;
using YourExam.Application.Interfaces;
using YourExam.Application.Services;
using System.Text.Json;

namespace YourExam.Application.Features.Exercises.Commands.GenerateExercise;

public class GenerateExerciseCommandHandler : IRequestHandler<GenerateExerciseCommand, GenerateExerciseResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IVariableGeneratorService _variableGenerator;
    private readonly IMathEvaluatorService _mathEvaluator;

    public GenerateExerciseCommandHandler(
        IApplicationDbContext dbContext,
        IVariableGeneratorService variableGenerator,
        IMathEvaluatorService mathEvaluator)
    {
        _dbContext = dbContext;
        _variableGenerator = variableGenerator;
        _mathEvaluator = mathEvaluator;
    }

    public async Task<GenerateExerciseResponse> Handle(GenerateExerciseCommand request, CancellationToken cancellationToken)
    {
        var response = new GenerateExerciseResponse { Success = true };

        // 1. Fetch matching templates from DB. 
        // Lấy toàn bộ template phù hợp điều kiện để có list dự phòng random.
        var matchingTemplates = await _dbContext.QuestionTemplates
            .Where(t => t.IsActive 
                        && t.Subject.ToLower() == request.Subject.ToLower()
                        && t.Difficulty == request.Difficulty
                        && t.QuestionType == request.QuestionType)
            .ToListAsync(cancellationToken);

        if (!matchingTemplates.Any())
        {
            return response;
        }

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
                // 2. Sinh biến số ngẫu nhiên (sẽ retry 50 lần bên trong Service)
                var variables = _variableGenerator.GenerateVariables(template.VariablesConfig, request.Subject, request.GradeLevel);

                // 3. Thay thế biến vào ContentTemplate
                string content = template.ContentTemplate;
                foreach (var kvp in variables)
                {
                    content = content.Replace("{" + kvp.Key + "}", kvp.Value.ToString());
                }

                // 4. Tính toán Đáp án đúng
                double correctAnswerRaw = _mathEvaluator.EvaluateMathExpression(template.AnswerFormula, variables);
                string correctAnswer = correctAnswerRaw.ToString();

                // 5. Tính toán Đáp án nhiễu
                var choices = new List<string> { correctAnswer };
                if (!string.IsNullOrWhiteSpace(template.DistractorLogic) && template.DistractorLogic != "[]")
                {
                    try
                    {
                        var distractorList = JsonSerializer.Deserialize<List<string>>(template.DistractorLogic);
                        if (distractorList != null)
                        {
                            foreach (var formula in distractorList)
                            {
                                double distractorRaw = _mathEvaluator.EvaluateMathExpression(formula, variables);
                                choices.Add(distractorRaw.ToString());
                            }
                        }
                    }
                    catch
                    {
                        // Ignore parsing errors for distractors
                    }
                }

                // Xóa đáp án trùng lặp và trộn (Shuffle Choices)
                choices = choices.Distinct().OrderBy(x => random.Next()).ToList();

                // 6. Thêm vào kết quả
                response.Data.Add(new GeneratedExerciseDto
                {
                    TemplateId = template.Id,
                    Content = content,
                    CorrectAnswer = correctAnswer,
                    Choices = choices,
                    GeneratedVariables = variables
                });

                generatedCount++;
            }
            catch (GenerateFailedException)
            {
                // BẮT LỖI RANDOM 50 LẦN THẤT BẠI:
                // Nếu code chạy vào đây, vòng lặp while sẽ tự động continue sang template tiếp theo (templateIndex++).
                // Đây chính là logic bạn yêu cầu: "random 50 lần vẫn k thỏa mãn thì chọn template khác".
                continue;
            }
        }

        return response;
    }
}
