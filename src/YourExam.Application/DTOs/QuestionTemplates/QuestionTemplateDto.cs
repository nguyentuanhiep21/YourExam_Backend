using YourExam.Domain.Enums;

namespace YourExam.Application.DTOs.QuestionTemplates;

public record QuestionTemplateDto(
    int Id,
    string Subject,
    int GradeLevel,
    string Topic,
    /// <summary>
    /// Mức độ khó của câu hỏi (1: Dễ, 2: Trung bình, 3: Khó).
    /// </summary>
    int Difficulty,
    /// <summary>
    /// Dạng bài tập toán học.
    /// </summary>
    ExerciseType ExerciseType,
    string ContentTemplate,
    /// <summary>
    /// Chuỗi JSON định nghĩa biến số. 
    /// Khi gọi API sinh đề, LLM trả về JSON Object, backend dùng JsonElement để nhận rồi lưu thành chuỗi này.
    /// </summary>
    string VariablesConfig,
    string AnswerFormula,
    /// <summary>
    /// Chuỗi JSON mảng các công thức nhiễu.
    /// Khi gọi API sinh đề, LLM trả về JSON Array, backend dùng JsonElement để nhận rồi lưu thành chuỗi này.
    /// </summary>
    string DistractorLogic,
    string? EqualTargetVariable,
    string MasterTemplateId,
    bool IsActive
);
