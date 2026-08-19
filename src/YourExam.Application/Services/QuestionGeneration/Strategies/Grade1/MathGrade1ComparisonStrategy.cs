using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YourExam.Domain.Entities;
using YourExam.Application.Interfaces;
using YourExam.Application.Features.Exercises.Commands.GenerateExercise;

namespace YourExam.Application.Services.QuestionGeneration.Strategies.Grade1;

public class MathGrade1ComparisonStrategy : IQuestionGeneratorStrategy
{
    private readonly IVariableGeneratorService _variableGenerator;
    private readonly IMathEvaluatorService _mathEvaluator;

    public MathGrade1ComparisonStrategy(
        IVariableGeneratorService variableGenerator,
        IMathEvaluatorService mathEvaluator)
    {
        _variableGenerator = variableGenerator;
        _mathEvaluator = mathEvaluator;
    }

    public bool CanHandle(string subject, int gradeLevel, Domain.Enums.ExerciseType exerciseType)
    {
        return subject.Equals("toan", StringComparison.OrdinalIgnoreCase) 
            && gradeLevel == 1 
            && exerciseType == Domain.Enums.ExerciseType.Comparison; // 3 = Comparison
    }

    public Task<GeneratedExerciseDto> GenerateAsync(QuestionTemplate template, Domain.Enums.QuestionFormat format)
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
                    
                    string pattern1 = @"\{\s*" + System.Text.RegularExpressions.Regex.Escape(key) + @"\s*\}";
                    string pattern2 = @"\[\s*" + System.Text.RegularExpressions.Regex.Escape(key) + @"\s*\]";

                    content = System.Text.RegularExpressions.Regex.Replace(content, pattern1, valueStr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    content = System.Text.RegularExpressions.Regex.Replace(content, pattern2, valueStr, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }

                // 3. Tính toán Đáp án đúng bằng logic trả về chuỗi thay vì double
                string correctAnswer = _mathEvaluator.EvaluateStringExpression(template.AnswerFormula, variables);
                
                // Đảm bảo đáp án đúng phải thuộc tập hợp >, <, =
                if (correctAnswer != ">" && correctAnswer != "<" && correctAnswer != "=")
                {
                    throw new Exception($"Công thức AnswerFormula không sinh ra kết quả hợp lệ (>, <, =). Giá trị sinh ra: {correctAnswer}");
                }

                // 4. Lấy tập hợp đáp án tĩnh
                var allOptions = new List<string> { ">", "<", "=" };

                // Tạo mảng choices bắt đầu bằng đáp án đúng
                var choices = new List<string> { correctAnswer };
                
                // Lấy ra các phương án nhiễu bằng cách loại bỏ đáp án đúng
                var distractors = allOptions.Where(op => op != correctAnswer).ToList();
                
                // Thêm các phương án nhiễu vào mảng choices
                choices.AddRange(distractors);

                // Xóa đáp án trùng lặp (nếu có)
                choices = choices.Distinct().ToList();

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
                // Lỗi sinh biến, lỗi parse JSON...
                // -> Vòng lặp sẽ tự động quay lại để gen biến số mới
            }
        }

        throw new Exception($"Không thể tạo câu hỏi Comparison từ template ID {template.Id} sau {maxRenderAttempts} lần thử. Vui lòng kiểm tra lại cấu hình JSON.");
    }
}
