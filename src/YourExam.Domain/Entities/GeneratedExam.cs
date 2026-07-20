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
    public User Author { get; set; } = null!;
    
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
    /// Đường dẫn tải file Microsoft Word (.docx) lưu trên Supabase Storage.
    /// </summary>
    public string DocxFileUrl { get; set; } = string.Empty;

    /// <summary>
    /// Đường dẫn tải file PDF xem trước (Tùy chọn, nếu hệ thống có chức năng convert PDF).
    /// </summary>
    public string? PdfFileUrl { get; set; }
    
    /// <summary>
    /// Tổng số lượt Upvote (Thích) cập nhật trực tiếp thay vì đếm động từ bảng Vote để tối ưu hiệu năng.
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

    // --- Navigation properties (phục vụ Entity Framework) ---
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
