namespace YourExam.Domain.Entities;

public class UserDocument
{
    /// <summary>
    /// Khóa chính tự tăng.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tên file hiển thị (VD: De_thi_toan.pdf).
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Đường dẫn tải/hiển thị file từ hệ thống cloud (Supabase Storage).
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    /// Định dạng file (pdf, doc, docx).
    /// </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// Kích thước file (tính theo bytes).
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// ID của người dùng đã upload file.
    /// </summary>
    public Guid AuthorId { get; set; }
    
    /// <summary>
    /// Navigation property: Người dùng upload file.
    /// </summary>
    public Profile Author { get; set; } = null!;

    /// <summary>
    /// Thời gian upload file.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
