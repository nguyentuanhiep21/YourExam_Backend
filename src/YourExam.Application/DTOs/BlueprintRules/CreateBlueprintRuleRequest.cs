using YourExam.Domain.Enums;

namespace YourExam.Application.DTOs.BlueprintRules;

public record CreateBlueprintRuleRequest(
    string Topic,
    int Difficulty,
    QuestionFormat QuestionFormat,
    ExerciseType ExerciseType,
    int Quantity
);
