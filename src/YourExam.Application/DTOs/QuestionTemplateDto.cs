namespace YourExam.Application.DTOs;

public class QuestionTemplateDto
{
    public int Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public int QuestionType { get; set; }
    public string ContentTemplate { get; set; } = string.Empty;
    
    // We can keep it as string for the DTO for now, or parse it to a specific object.
    // Since the API client will consume it, returning raw JSON string or parsed object depends on preference.
    // For simplicity, we'll return the string and let the frontend parse it.
    public string VariablesConfig { get; set; } = "{}";
    
    public string AnswerFormula { get; set; } = string.Empty;
    public string DistractorLogic { get; set; } = "[]";
    public bool IsActive { get; set; }
}
