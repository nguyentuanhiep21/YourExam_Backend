namespace YourExam.Application.DTOs;

public class QuestionTemplateDto
{
    public int Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public string Topic { get; set; } = string.Empty;
    /// <summary>
    /// Mức độ khó của câu hỏi (1: Dễ, 2: Trung bình, 3: Khó).
    /// </summary>
    public int Difficulty { get; set; }
    
    /// <summary>
    /// Dạng bài tập toán học.
    /// </summary>
    public Domain.Enums.ExerciseType ExerciseType { get; set; }
    
    public string ContentTemplate { get; set; } = string.Empty;
    
    /// <summary>
    /// Chuỗi JSON định nghĩa biến số. 
    /// Khi gọi API sinh đề, LLM trả về JSON Object, backend dùng JsonElement để nhận rồi lưu thành chuỗi này.
    /// </summary>
    public string VariablesConfig { get; set; } = "{}";
    
    public string AnswerFormula { get; set; } = string.Empty;
    
    /// <summary>
    /// Chuỗi JSON mảng các công thức nhiễu.
    /// Khi gọi API sinh đề, LLM trả về JSON Array, backend dùng JsonElement để nhận rồi lưu thành chuỗi này.
    /// </summary>
    public string DistractorLogic { get; set; } = string.Empty;
    public string? EqualTargetVariable { get; set; }
    public string MasterTemplateId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
