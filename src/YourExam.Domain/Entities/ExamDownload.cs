namespace YourExam.Domain.Entities;

public class ExamDownload
{
    /// <summary>
    /// Khóa ngoại trỏ về Đề thi được tải. Đóng vai trò là 1 phần của Composite Primary Key.
    /// </summary>
    public int ExamId { get; set; }
    public GeneratedExam Exam { get; set; } = null!;
    
    /// <summary>
    /// Khóa ngoại trỏ về User đã tải. Đóng vai trò là phần còn lại của Composite Primary Key.
    /// </summary>
    public Guid UserId { get; set; }
    public Profile User { get; set; } = null!;
    
    /// <summary>
    /// Thời gian tải xuống lần đầu.
    /// </summary>
    public DateTime DownloadedAt { get; set; } = DateTime.UtcNow;
}
