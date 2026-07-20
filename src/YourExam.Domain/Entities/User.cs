namespace YourExam.Domain.Entities;

public class User
{
    /// <summary>
    /// Khóa chính của bảng. Sử dụng Guid để map trực tiếp 1-1 với ID của Supabase Auth.
    /// </summary>
    public Guid Id { get; set; } 

    /// <summary>
    /// Địa chỉ email đăng nhập của người dùng.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Họ và tên hiển thị của giáo viên/người dùng.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Tên trường nơi giáo viên đang công tác (Tùy chọn).
    /// </summary>
    public string? School { get; set; }

    /// <summary>
    /// Danh sách các môn học giáo viên đang giảng dạy (Tùy chọn).
    /// </summary>
    public string? SubjectsTaught { get; set; }

    /// <summary>
    /// Đường dẫn (URL) tới ảnh đại diện của người dùng.
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Thời gian tạo tài khoản.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // --- Navigation properties (phục vụ Entity Framework) ---
    public ICollection<GeneratedExam> GeneratedExams { get; set; } = new List<GeneratedExam>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
