namespace YourExam.Domain.Entities;

public class ExamBlueprint
{
    /// <summary>
    /// Khóa chính tự tăng.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tên của khung đề thi (Ví dụ: "Đề Toán Cuối Kỳ 1 - Lớp 3").
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Đánh dấu xem đây là Blueprint do Hệ thống cung cấp mặc định (ai cũng dùng được) 
    /// hay là Blueprint do cá nhân giáo viên tự tạo.
    /// </summary>
    public bool IsSystemProvided { get; set; }
    
    /// <summary>
    /// Khóa ngoại trỏ về User (người tạo ra Blueprint này). 
    /// Dùng Guid để map chuẩn với bảng USERS.
    /// </summary>
    public Guid AuthorId { get; set; } 
    
    // --- Navigation properties (phục vụ Entity Framework) ---
    
    /// <summary>
    /// Danh sách các Luật (Rule) cấu thành nên Ma trận của đề thi này.
    /// </summary>
    public ICollection<BlueprintRule> Rules { get; set; } = new List<BlueprintRule>();

    /// <summary>
    /// Danh sách các Đề Thi thực tế đã được gen ra dựa trên Blueprint này.
    /// </summary>
    public ICollection<GeneratedExam> GeneratedExams { get; set; } = new List<GeneratedExam>();
}
