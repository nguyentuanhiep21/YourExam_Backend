using MediatR;

namespace YourExam.Application.Features.Exercises.Commands.GenerateExercise;

public class GenerateExerciseCommand : IRequest<GenerateExerciseResponse>
{
    public string Subject { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public Domain.Enums.ExerciseType ExerciseType { get; set; }
    public int GradeLevel { get; set; }
    public string? Topic { get; set; }
    public int Quantity { get; set; } = 1;
}

public class GenerateExerciseResponse
{
    public bool Success { get; set; }
    public List<GeneratedExerciseDto> Data { get; set; } = new();
}

public class GeneratedExerciseDto
{
    public int TemplateId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string> Choices { get; set; } = new();
    public string CorrectAnswer { get; set; } = string.Empty;
    public Dictionary<string, double> GeneratedVariables { get; set; } = new();
}
