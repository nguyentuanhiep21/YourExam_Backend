namespace YourExam.Domain.Entities;

public class Topic
{
    /// <summary>
    /// Khóa chính tự tăng.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tiêu đề của topic hỏi đáp.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Nội dung chi tiết của topic.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Đường dẫn tới ảnh đính kèm (nếu có).
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Khóa ngoại trỏ về User (Người đăng topic).
    /// </summary>
    public Guid AuthorId { get; set; }
    public Profile Author { get; set; } = null!;

    /// <summary>
    /// Thời gian tạo topic.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Số lượng comment của topic.
    /// </summary>
    public int CommentCount { get; set; } = 0;

    // --- Navigation properties (phục vụ Entity Framework) ---
    public ICollection<TopicComment> Comments { get; set; } = new List<TopicComment>();
    public ICollection<SavedTopic> SavedTopics { get; set; } = new List<SavedTopic>();
}
