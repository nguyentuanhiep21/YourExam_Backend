namespace YourExam.Domain.Entities;

public class SavedTopic
{
    /// <summary>
    /// Khóa chính tự tăng.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Khóa ngoại trỏ về Topic được lưu.
    /// </summary>
    public int TopicId { get; set; }
    public Topic Topic { get; set; } = null!;

    /// <summary>
    /// Khóa ngoại trỏ về Profile (Người lưu topic).
    /// </summary>
    public Guid UserId { get; set; }
    public Profile User { get; set; } = null!;

    /// <summary>
    /// Thời gian lưu topic.
    /// </summary>
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}
