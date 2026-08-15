using MediatR;
using YourExam.Application.DTOs.Export;

namespace YourExam.Application.Features.Exercises.Commands.ExportExercisesToDocx;

public record ExportExercisesToDocxCommand(
    string FileName,
    List<ExportExerciseItemDto> Exercises
) : IRequest<ExportExercisesResponse>;

public record ExportExercisesResponse(
    byte[] ZipBytes,
    string ZipFileName
);
