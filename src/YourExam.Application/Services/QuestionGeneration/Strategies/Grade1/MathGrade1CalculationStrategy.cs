using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using YourExam.Domain.Entities;
using YourExam.Application.Interfaces;
using YourExam.Application.Features.Exercises.Commands.GenerateExercise;

namespace YourExam.Application.Services.QuestionGeneration.Strategies.Grade1;

public class MathGrade1CalculationStrategy : IQuestionGeneratorStrategy
{
    private readonly IVariableGeneratorService _variableGenerator;
    private readonly IMathEvaluatorService _mathEvaluator;

    public MathGrade1CalculationStrategy(
        IVariableGeneratorService variableGenerator,
        IMathEvaluatorService mathEvaluator)
    {
        _variableGenerator = variableGenerator;
        _mathEvaluator = mathEvaluator;
    }

    public bool CanHandle(string subject, int gradeLevel, int questionType)
    {
        return subject.Equals("Toán", StringComparison.OrdinalIgnoreCase) 
            && gradeLevel == 1 
            && questionType == 1; // 1 = Calculation
    }

    public Task<GeneratedExerciseDto> GenerateAsync(QuestionTemplate template)
    {
        var random = new Random();
        bool successForTemplate = false;
        int renderAttempts = 0;
        int maxRenderAttempts = 10;

        while (!successForTemplate && renderAttempts < maxRenderAttempts)
        {
            renderAttempts++;
            try
            {
                // 1. Sinh biến số ngẫu nhiên
                var variables = _variableGenerator.GenerateVariables(template.VariablesConfig, template.Subject, template.GradeLevel);

                // 2. Thay thế biến vào ContentTemplate
                string content = template.ContentTemplate;
                foreach (var kvp in variables)
                {
                    string key = kvp.Key.Trim();
                    string valueStr = kvp.Value.ToString();
                    
                    // Regex tìm kiếm ngoặc nhọn hoặc ngoặc vuông có thể chứa khoảng trắng dư thừa, ví dụ: [ a ], {a }
                    string pattern1 = @"\{\s*" + System.Text.RegularExpressions.Regex.Escape(key) + @"\s*\}";
                    string pattern2 = @"\[\s*" + System.Text.RegularExpressions.Regex.Escape(key) + @"\s*\]";

                    content = System.Text.RegularExpressions.Regex.Replace(content, pattern1, valueStr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    content = System.Text.RegularExpressions.Regex.Replace(content, pattern2, valueStr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }

                // 3. Tính toán Đáp án đúng
                double correctAnswerRaw = _mathEvaluator.EvaluateMathExpression(template.AnswerFormula, variables);
                string correctAnswer = correctAnswerRaw.ToString();

                // 4. Tính toán Đáp án nhiễu
                var choices = new List<string> { correctAnswer };
                if (!string.IsNullOrWhiteSpace(template.DistractorLogic) && template.DistractorLogic != "[]")
                {
                    var options = new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip };
                    var distractorList = JsonSerializer.Deserialize<List<string>>(template.DistractorLogic, options);
                    if (distractorList != null)
                    {
                        foreach (var formula in distractorList)
                        {
                            double distractorRaw = _mathEvaluator.EvaluateMathExpression(formula, variables);
                            choices.Add(distractorRaw.ToString());
                        }
                    }
                }

                // Xóa đáp án trùng lặp
                choices = choices.Distinct().ToList();

                // Gen thêm đáp án nhiễu ngẫu nhiên nếu chưa đủ 4 đáp án (Backfill)
                while (choices.Count < 4)
                {
                    // Random lệch từ -5 đến +5 so với đáp án đúng
                    int offset = random.Next(-5, 6);
                    if (offset == 0) offset = 6; 

                    double generatedDistractor = correctAnswerRaw + offset;
                    
                    // Lớp 1 không có đáp án âm
                    if (generatedDistractor < 0) 
                    {
                        generatedDistractor = Math.Abs(generatedDistractor);
                        // Đề phòng Math.Abs bị trùng lại
                        if (generatedDistractor == correctAnswerRaw) generatedDistractor += 1;
                    }

                    string newChoice = generatedDistractor.ToString();
                    if (!choices.Contains(newChoice))
                    {
                        choices.Add(newChoice);
                    }
                }

                // Trộn (Shuffle Choices)
                choices = choices.OrderBy(x => random.Next()).ToList();
                // 5. Trả về kết quả
                return Task.FromResult(new GeneratedExerciseDto
                {
                    TemplateId = template.Id,
                    Content = content,
                    CorrectAnswer = correctAnswer,
                    Choices = choices,
                    GeneratedVariables = variables
                });
            }
            catch (Exception)
            {
                // Lỗi sinh biến, lỗi Toán học (chia cho 0), lỗi parse JSON nhiễu...
                // -> Vòng lặp sẽ tự động quay lại để gen biến số mới
            }
        }

        throw new Exception($"Không thể tạo câu hỏi từ template ID {template.Id} sau {maxRenderAttempts} lần thử. Vui lòng kiểm tra lại cấu hình JSON.");
    }
}
