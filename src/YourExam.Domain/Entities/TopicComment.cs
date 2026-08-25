namespace YourExam.Domain.Entities;

public class TopicComment
{
    /// <summary>
    /// Khóa chính tự tăng.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Khóa ngoại trỏ về Topic mà bình luận này thuộc về.
    /// </summary>
    public int TopicId { get; set; }
    public Topic Topic { get; set; } = null!;

    /// <summary>
    /// Nội dung của bình luận/câu trả lời.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Khóa ngoại trỏ về User (Người đăng bình luận).
    /// </summary>
    public Guid AuthorId { get; set; }
    public Profile Author { get; set; } = null!;

    /// <summary>
    /// Thời gian tạo bình luận.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
