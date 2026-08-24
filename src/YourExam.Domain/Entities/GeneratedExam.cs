namespace YourExam.Domain.Entities;

public class GeneratedExam
{
    /// <summary>
    /// Khóa chính tự tăng.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Khóa ngoại trỏ về User (Người đã bấm nút tạo đề thi này).
    /// </summary>
    public Guid AuthorId { get; set; }
    public Profile Author { get; set; } = null!;
    
    /// <summary>
    /// Khóa ngoại trỏ về Blueprint (Đề thi này được gen ra từ khung nào).
    /// </summary>
    public int BlueprintId { get; set; }
    public ExamBlueprint Blueprint { get; set; } = null!;
    
    /// <summary>
    /// Tên hiển thị của đề thi (Ví dụ: "Đề kiểm tra 15 phút Toán").
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Khối lớp áp dụng (Ví dụ: 1, 2, 3, 4, 5).
    /// </summary>
    public int GradeLevel { get; set; }

    /// <summary>
    /// Môn học (Ví dụ: "Toán học", "Ngữ văn", "Tiếng Anh").
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Thời gian làm bài tính bằng phút (Ví dụ: 45, 90).
    /// </summary>
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Tổng điểm của đề thi (Ví dụ: 10, 20, 100).
    /// </summary>
    public decimal TotalScore { get; set; }

    /// <summary>
    /// Mức độ khó tổng thể của đề thi (1: Dễ, 2: Trung bình, 3: Khó).
    /// </summary>
    public int Difficulty { get; set; }

    /// <summary>
    /// Đường dẫn tải file Microsoft Word (.docx) lưu trên Supabase Storage.
    /// </summary>
    public string DocxFileUrl { get; set; } = string.Empty;

    /// <summary>
    /// Đường dẫn tải file PDF xem trước (Tùy chọn, nếu hệ thống có chức năng convert PDF).
    /// </summary>
    public string? PdfFileUrl { get; set; }
    
    /// <summary>
    /// Tổng số lượt Upvote (Thích) cập nhật trực tiếp thay vì đếm động từ bảng ExamVote để tối ưu hiệu năng.
    /// </summary>
    public int UpvoteCount { get; set; } = 0;

    /// <summary>
    /// Tổng số lượt Downvote (Không thích).
    /// </summary>
    public int DownvoteCount { get; set; } = 0;

    /// <summary>
    /// Tổng số lượt người dùng đã bấm nút Tải xuống.
    /// </summary>
    public int DownloadCount { get; set; } = 0;
    
    /// <summary>
    /// Cờ đánh dấu đề thi này được công khai cho cộng đồng hay chỉ lưu riêng tư.
    /// </summary>
    public bool IsPublic { get; set; } = true;

    /// <summary>
    /// Thời gian tạo đề.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời gian cập nhật gần nhất.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // --- Navigation properties (phục vụ Entity Framework) ---
    public ICollection<GeneratedExamQuestion> Questions { get; set; } = new List<GeneratedExamQuestion>();
    public ICollection<ExamVote> ExamVotes { get; set; } = new List<ExamVote>();
    public ICollection<ExamDownload> Downloads { get; set; } = new List<ExamDownload>();
}
