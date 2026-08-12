namespace YourExam.Application.Services.QuestionGeneration;

public interface IQuestionGeneratorFactory
{
    IQuestionGeneratorStrategy GetStrategy(string subject, int gradeLevel, Domain.Enums.ExerciseType exerciseType);
}
