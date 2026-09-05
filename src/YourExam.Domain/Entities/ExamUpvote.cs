namespace YourExam.Domain.Entities;

public class ExamUpvote
{
    /// <summary>
    /// Khóa ngoại trỏ về Đề thi được upvote.
    /// </summary>
    public int ExamId { get; set; }
    public GeneratedExam Exam { get; set; } = null!;
    
    /// <summary>
    /// Khóa ngoại trỏ về User đã upvote.
    /// </summary>
    public Guid UserId { get; set; }
    public Profile User { get; set; } = null!;
    
    /// <summary>
    /// Thời gian thực hiện thao tác Upvote.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
