namespace YourExam.Application.DTOs.Export;

public record ExportExerciseItemDto(
    string Content,
    List<string> Choices,
    string CorrectAnswer
);
