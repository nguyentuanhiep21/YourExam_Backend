using YourExam.Domain.Entities;
using YourExam.Application.Features.Exercises.Commands.GenerateExercise;
using System.Threading.Tasks;

namespace YourExam.Application.Services.QuestionGeneration;

public interface IQuestionGeneratorStrategy
{
    bool CanHandle(string subject, int gradeLevel, int questionType);
    Task<GeneratedExerciseDto> GenerateAsync(QuestionTemplate template);
}
