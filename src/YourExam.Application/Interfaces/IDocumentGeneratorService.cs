using YourExam.Application.DTOs.Export;

namespace YourExam.Application.Interfaces;

public interface IDocumentGeneratorService
{
    Task<byte[]> GenerateExamZipAsync(string fileName, List<ExportExerciseItemDto> exercises);
}
