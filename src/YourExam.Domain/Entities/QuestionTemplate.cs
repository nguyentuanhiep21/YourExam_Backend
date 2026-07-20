namespace YourExam.Domain.Entities;

public class QuestionTemplate
{
    /// <summary>
    /// Khóa chính tự tăng.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Môn học áp dụng (Ví dụ: "Toán", "Tiếng Việt").
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Tên chủ đề kiến thức (Ví dụ: "Phép cộng trừ phạm vi 100", "Hình học").
    /// </summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// Mức độ khó của câu hỏi (Ví dụ: 1: Dễ, 2: Trung bình, 3: Khó).
    /// </summary>
    public int Difficulty { get; set; } 

    /// <summary>
    /// Dạng bài tập (Ví dụ: 1: Trắc nghiệm, 2: Tự luận, 3: Đặt tính rồi tính).
    /// </summary>
    public int QuestionType { get; set; } 

    /// <summary>
    /// Khung nội dung câu hỏi chứa các biến. Ví dụ: "Lan có {x} quả táo, Lan cho Hoa {y} quả táo. Hỏi Lan còn bao nhiêu quả?".
    /// </summary>
    public string ContentTemplate { get; set; } = string.Empty;
    
    /// <summary>
    /// Chuỗi JSON định nghĩa quy tắc random các biến số (Ví dụ: x nằm từ 10->50, y nằm từ 1->x).
    /// Postgres sẽ tự map chuỗi này thành kiểu jsonb.
    /// </summary>
    public string VariablesConfig { get; set; } = "{}";
    
    /// <summary>
    /// Công thức toán học để tính ra đáp án cuối cùng dựa vào các biến số. Ví dụ: "{x} - {y}".
    /// </summary>
    public string AnswerFormula { get; set; } = string.Empty;
    
    /// <summary>
    /// Chuỗi JSON định nghĩa thuật toán sinh ra các đáp án gây nhiễu (Dành cho câu trắc nghiệm).
    /// </summary>
    public string DistractorLogic { get; set; } = "[]";
    
    /// <summary>
    /// Cờ đánh dấu Template này có đang được sử dụng hay không.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
