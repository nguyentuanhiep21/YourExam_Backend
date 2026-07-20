namespace YourExam.Domain.Entities;

public class Vote
{
    /// <summary>
    /// Khóa ngoại trỏ về Đề thi được vote. Đóng vai trò là 1 phần của Composite Primary Key.
    /// </summary>
    public int ExamId { get; set; }
    public GeneratedExam Exam { get; set; } = null!;
    
    /// <summary>
    /// Khóa ngoại trỏ về User đã vote. Đóng vai trò là phần còn lại của Composite Primary Key.
    /// </summary>
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    /// <summary>
    /// Loại Vote: 'true' nghĩa là Upvote (Thích), 'false' nghĩa là Downvote (Không thích).
    /// Sự tồn tại của dòng này trong DB chứng tỏ User đã vote. Nếu chưa vote, DB sẽ không có record nào.
    /// </summary>
    public bool IsUpvote { get; set; } 
    
    /// <summary>
    /// Thời gian thực hiện thao tác Vote.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
