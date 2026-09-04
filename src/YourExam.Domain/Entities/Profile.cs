namespace YourExam.Domain.Entities;

public class Profile
{
    /// <summary>
    /// Khóa chính của bảng. Dùng Guid để map trực tiếp 1-1 với ID của Supabase Auth.
    /// </summary>
    public Guid Id { get; set; } 


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
    public ICollection<ExamVote> ExamVotes { get; set; } = new List<ExamVote>();
    public ICollection<ExamDownload> ExamDownloads { get; set; } = new List<ExamDownload>();
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public ICollection<TopicComment> TopicComments { get; set; } = new List<TopicComment>();
    public ICollection<SavedTopic> SavedTopics { get; set; } = new List<SavedTopic>();
    public ICollection<UserDocument> UserDocuments { get; set; } = new List<UserDocument>();
}
