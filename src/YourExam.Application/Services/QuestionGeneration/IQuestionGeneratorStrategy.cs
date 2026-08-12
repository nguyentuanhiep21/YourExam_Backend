using YourExam.Domain.Entities;
using YourExam.Application.Features.Exercises.Commands.GenerateExercise;
using System.Threading.Tasks;

namespace YourExam.Application.Services.QuestionGeneration;

public interface IQuestionGeneratorStrategy
{
    bool CanHandle(string subject, int gradeLevel, Domain.Enums.ExerciseType exerciseType);
    Task<GeneratedExerciseDto> GenerateAsync(QuestionTemplate template);
}
