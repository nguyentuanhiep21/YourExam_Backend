using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using YourExam.Domain.Entities;
using YourExam.Application.Interfaces;
using YourExam.Application.Features.Exercises.Commands.GenerateExercise;

namespace YourExam.Application.Services.QuestionGeneration.Strategies.Grade1;

public class MathGrade1FillInTheBlankStrategy : IQuestionGeneratorStrategy
{
    private readonly IVariableGeneratorService _variableGenerator;
    private readonly IMathEvaluatorService _mathEvaluator;

    public MathGrade1FillInTheBlankStrategy(
        IVariableGeneratorService variableGenerator,
        IMathEvaluatorService mathEvaluator)
    {
        _variableGenerator = variableGenerator;
        _mathEvaluator = mathEvaluator;
    }

    public bool CanHandle(string subject, int gradeLevel, Domain.Enums.ExerciseType exerciseType)
    {
        return subject.Equals("Toán", StringComparison.OrdinalIgnoreCase) 
            && gradeLevel == 1 
            && exerciseType == Domain.Enums.ExerciseType.FillInTheBlank;
    }

    public Task<GeneratedExerciseDto> GenerateAsync(QuestionTemplate template, Domain.Enums.QuestionFormat format)
    {
        var random = new Random();
        int renderAttempts = 0;
        int maxRenderAttempts = 50;

        while (renderAttempts < maxRenderAttempts)
        {
            renderAttempts++;
            try
            {
                // 1. Sinh biến số ngẫu nhiên theo constraints
                var variables = _variableGenerator.GenerateVariables(template.VariablesConfig, template.Subject, template.GradeLevel);

                // 2. Ép biến bị ẩn (EqualTargetVariable) bằng giá trị của AnswerFormula
                //    để đảm bảo 2 vế của phương trình luôn bằng nhau.
                if (!string.IsNullOrWhiteSpace(template.EqualTargetVariable))
                {
                    double forcedValue = _mathEvaluator.EvaluateMathExpression(template.AnswerFormula, variables);

                    // Đáp án phải là số nguyên dương
                    if (forcedValue <= 0 || forcedValue != Math.Floor(forcedValue))
                        continue;

                    variables[template.EqualTargetVariable] = forcedValue;
                }

                // 3. Tính đáp án đúng
                double correctAnswerRaw = _mathEvaluator.EvaluateMathExpression(template.AnswerFormula, variables);

                if (correctAnswerRaw <= 0 || correctAnswerRaw != Math.Floor(correctAnswerRaw))
                    continue;

                string correctAnswer = ((int)correctAnswerRaw).ToString();

                // 3b. Validate: các biến hiển thị trong content phải có giá trị riêng biệt
                //     VÀ đáp án không được bằng bất kỳ biến hiển thị nào (tránh đáp án lộ rõ).
                var visibleValues = variables
                    .Where(kvp => !kvp.Key.Equals(template.EqualTargetVariable ?? "", StringComparison.OrdinalIgnoreCase))
                    .Select(kvp => kvp.Value)
                    .ToList();

                bool hasDuplicateVisible = visibleValues.Count != visibleValues.Distinct().Count();
                bool answerRevealedByContent = visibleValues.Contains(correctAnswerRaw);

                if (hasDuplicateVisible || answerRevealedByContent)
                    continue;

                // 4. Thay thế biến vào ContentTemplate (bỏ qua EqualTargetVariable — đó là chỗ trống "...")
                string content = template.ContentTemplate;
                foreach (var kvp in variables)
                {
                    // Không thay thế biến bị ẩn, vì trong template nó được biểu thị bằng "..."
                    if (!string.IsNullOrWhiteSpace(template.EqualTargetVariable)
                        && kvp.Key.Equals(template.EqualTargetVariable, StringComparison.OrdinalIgnoreCase))
                        continue;

                    string key = kvp.Key.Trim();
                    string valueStr = ((int)kvp.Value).ToString();

                    string pattern1 = @"\{\s*" + System.Text.RegularExpressions.Regex.Escape(key) + @"\s*\}";
                    string pattern2 = @"\[\s*" + System.Text.RegularExpressions.Regex.Escape(key) + @"\s*\]";

                    content = System.Text.RegularExpressions.Regex.Replace(content, pattern1, valueStr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    content = System.Text.RegularExpressions.Regex.Replace(content, pattern2, valueStr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }

                // 5. Tính các đáp án nhiễu từ DistractorLogic
                var choices = new List<string> { correctAnswer };

                if (!string.IsNullOrWhiteSpace(template.DistractorLogic) && template.DistractorLogic != "[]")
                {
                    var jsonOptions = new JsonSerializerOptions { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip };
                    var distractorFormulas = JsonSerializer.Deserialize<List<string>>(template.DistractorLogic, jsonOptions);
                    if (distractorFormulas != null)
                    {
                        foreach (var formula in distractorFormulas)
                        {
                            double distractorRaw = _mathEvaluator.EvaluateMathExpression(formula, variables);
                            if (distractorRaw > 0 && distractorRaw == Math.Floor(distractorRaw))
                            {
                                choices.Add(((int)distractorRaw).ToString());
                            }
                        }
                    }
                }

                // Xóa trùng lặp
                choices = choices.Distinct().ToList();

                // Backfill nếu chưa đủ 4 đáp án
                while (choices.Count < 4)
                {
                    int offset = random.Next(-5, 6);
                    if (offset == 0) offset = random.Next(1, 6);

                    int candidate = (int)correctAnswerRaw + offset;
                    if (candidate <= 0) candidate = (int)correctAnswerRaw + Math.Abs(offset) + 1;

                    string candidateStr = candidate.ToString();
                    if (!choices.Contains(candidateStr))
                        choices.Add(candidateStr);
                }

                // Trộn (Shuffle)
                choices = choices.OrderBy(_ => random.Next()).ToList();

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
                // Lỗi sinh biến hoặc tính toán -> thử lại
            }
        }

        throw new Exception($"Không thể tạo câu hỏi FillInTheBlank từ template ID {template.Id} sau {maxRenderAttempts} lần thử. Vui lòng kiểm tra lại cấu hình JSON.");
    }
}
