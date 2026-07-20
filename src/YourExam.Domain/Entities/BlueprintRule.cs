namespace YourExam.Domain.Entities;

public class BlueprintRule
{
    /// <summary>
    /// Khóa chính tự tăng.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Khóa ngoại trỏ về Blueprint cha (Chứa ma trận này).
    /// </summary>
    public int BlueprintId { get; set; }
    public ExamBlueprint Blueprint { get; set; } = null!;
    
    /// <summary>
    /// Tên chủ đề cần bốc câu hỏi (Ví dụ: "Hình học cơ bản").
    /// </summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// Mức độ khó yêu cầu (Ví dụ: 1: Dễ, 2: Trung bình, 3: Khó).
    /// </summary>
    public int Difficulty { get; set; }

    /// <summary>
    /// Dạng bài yêu cầu (Ví dụ: 1: Trắc nghiệm, 2: Tự luận).
    /// </summary>
    public int QuestionType { get; set; }

    /// <summary>
    /// Số lượng câu hỏi ngẫu nhiên cần bốc thỏa mãn các tiêu chí trên.
    /// </summary>
    public int Quantity { get; set; }
}
