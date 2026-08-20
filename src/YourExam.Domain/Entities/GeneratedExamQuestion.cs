using YourExam.Domain.Enums;

namespace YourExam.Domain.Entities;

public class GeneratedExamQuestion
{
    public int Id { get; init; }

    // Foreign Keys
    public int GeneratedExamId { get; set; }
    public int? QuestionTemplateId { get; set; }

    // Question Details
    public int OrderIndex { get; set; }
    public QuestionType QuestionType { get; set; }
    public int Difficulty { get; set; }
    public string QuestionContent { get; set; } = string.Empty;

    // Multiple Choice Fields (nullable for Essay questions)
    public string? MultipleChoiceOptions { get; set; }  // JSON: ["A. ...", "B. ...", "C. ...", "D. ..."]
    public string? CorrectAnswer { get; set; }          // "A", "B", "C", "D" or null for Essay

    // Scoring
    public decimal Score { get; set; }

    // Optional Explanation
    public string? Explanation { get; set; }

    // Navigation Properties
    public GeneratedExam GeneratedExam { get; set; } = null!;
    public QuestionTemplate? QuestionTemplate { get; set; }
}
