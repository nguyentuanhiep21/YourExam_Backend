using MediatR;
using YourExam.Application.Interfaces;

namespace YourExam.Application.Features.Exercises.Commands.ExportExercisesToDocx;

public class ExportExercisesToDocxCommandHandler : IRequestHandler<ExportExercisesToDocxCommand, ExportExercisesResponse>
{
    private readonly IDocumentGeneratorService _documentGeneratorService;

    public ExportExercisesToDocxCommandHandler(IDocumentGeneratorService documentGeneratorService)
    {
        _documentGeneratorService = documentGeneratorService;
    }

    public async Task<ExportExercisesResponse> Handle(ExportExercisesToDocxCommand request, CancellationToken cancellationToken)
    {
        var safeFileName = string.IsNullOrWhiteSpace(request.FileName) ? "DeThi" : request.FileName;
        var zipBytes = await _documentGeneratorService.GenerateExamZipAsync(safeFileName, request.Exercises);
        
        return new ExportExercisesResponse(zipBytes, $"{safeFileName}.zip");
    }
}
