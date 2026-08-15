using YourExam.Domain.Enums;

namespace YourExam.Application.DTOs.BlueprintRules;

public record BlueprintRuleDto(
    int Id,
    int BlueprintId,
    string Topic,
    int Difficulty,
    QuestionFormat QuestionFormat,
    ExerciseType ExerciseType,
    int Quantity
);
