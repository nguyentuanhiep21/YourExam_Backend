using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using YourExam.Domain.Entities;
using YourExam.Application.Interfaces;
using YourExam.Application.Features.Exercises.Commands.GenerateExercise;

namespace YourExam.Application.Services.QuestionGeneration.Strategies.Grade1;

public class MathGrade1WordProblemStrategy : IQuestionGeneratorStrategy
{
    private readonly IVariableGeneratorService _variableGenerator;
    private readonly IMathEvaluatorService _mathEvaluator;
    private readonly ITextVariableGeneratorService _textVariableGenerator;

    public MathGrade1WordProblemStrategy(
        IVariableGeneratorService variableGenerator,
        IMathEvaluatorService mathEvaluator,
        ITextVariableGeneratorService textVariableGenerator)
    {
        _variableGenerator = variableGenerator;
        _mathEvaluator = mathEvaluator;
        _textVariableGenerator = textVariableGenerator;
    }

    public bool CanHandle(string subject, int gradeLevel, int questionType)
    {
        return subject.Equals("Toán", StringComparison.OrdinalIgnoreCase) 
            && gradeLevel == 1 
            && questionType == 2; // 2 = WordProblem
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

                // 2. Thay thế biến Text vào ContentTemplate (VD: [NhanVat] -> "An", [VatThe_HoaQua] -> "quả táo")
                string content = _textVariableGenerator.ReplaceTextVariables(template.ContentTemplate);

                // 3. Thay thế biến Số vào ContentTemplate
                foreach (var kvp in variables)
                {
                    string key = kvp.Key.Trim();
                    string valueStr = kvp.Value.ToString();
                    
                    // Regex tìm kiếm ngoặc vuông (vd: [a])
                    string pattern = @"\[\s*" + System.Text.RegularExpressions.Regex.Escape(key) + @"\s*\]";
                    content = System.Text.RegularExpressions.Regex.Replace(content, pattern, valueStr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }

                // 4. Tính toán Đáp án đúng
                double correctAnswerRaw = _mathEvaluator.EvaluateMathExpression(template.AnswerFormula, variables);
                string correctAnswer = correctAnswerRaw.ToString();

                // 5. Tính toán Đáp án nhiễu
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

                // Gen thêm đáp án nhiễu ngẫu nhiên nếu chưa đủ 4 đáp án
                while (choices.Count < 4)
                {
                    int offset = random.Next(-3, 4);
                    if (offset == 0) offset = 4; 

                    double generatedDistractor = correctAnswerRaw + offset;
                    
                    if (generatedDistractor < 0) 
                    {
                        generatedDistractor = Math.Abs(generatedDistractor);
                        if (generatedDistractor == correctAnswerRaw) generatedDistractor += 1;
                    }

                    string newChoice = generatedDistractor.ToString();
                    if (!choices.Contains(newChoice))
                    {
                        choices.Add(newChoice);
                    }
                }

                // Trộn
                choices = choices.OrderBy(x => random.Next()).ToList();
                
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
                // Vòng lặp sẽ tự động quay lại để gen biến số mới
            }
        }

        throw new Exception($"Không thể tạo câu hỏi từ template ID {template.Id} sau {maxRenderAttempts} lần thử. Vui lòng kiểm tra lại cấu hình JSON.");
    }
}
