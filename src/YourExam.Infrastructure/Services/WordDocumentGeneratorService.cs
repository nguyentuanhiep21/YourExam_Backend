using System.IO.Compression;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using YourExam.Application.DTOs.Export;
using YourExam.Application.Interfaces;

namespace YourExam.Infrastructure.Services;

public class WordDocumentGeneratorService : IDocumentGeneratorService
{
    public async Task<byte[]> GenerateExamZipAsync(string fileName, List<ExportExerciseItemDto> exercises)
    {
        // 1. Generate Exam Document (Questions only)
        byte[] examBytes = GenerateDocx(exercises, includeAnswers: false);

        // 2. Generate Answer Document (Questions + Correct Answers)
        byte[] answerBytes = GenerateDocx(exercises, includeAnswers: true);

        // 3. Create ZIP archive in memory
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var examEntry = archive.CreateEntry($"{fileName}.docx", CompressionLevel.Optimal);
            using (var entryStream = examEntry.Open())
            {
                await entryStream.WriteAsync(examBytes, 0, examBytes.Length);
            }

            var answerEntry = archive.CreateEntry($"{fileName}_DA.docx", CompressionLevel.Optimal);
            using (var entryStream = answerEntry.Open())
            {
                await entryStream.WriteAsync(answerBytes, 0, answerBytes.Length);
            }
        }

        return memoryStream.ToArray();
    }

    private byte[] GenerateDocx(List<ExportExerciseItemDto> exercises, bool includeAnswers)
    {
        using var memoryStream = new MemoryStream();
        using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
        {
            var mainPart = wordDocument.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            for (int i = 0; i < exercises.Count; i++)
            {
                var exercise = exercises[i];
                
                // Add Question Content
                var questionParagraph = new Paragraph(
                    new Run(
                        new RunProperties(new Bold()),
                        new Text($"Câu {i + 1}: ")
                    ),
                    new Run(
                        new Text(exercise.Content)
                    )
                );
                body.AppendChild(questionParagraph);

                // Add Choices if it's a multiple choice question
                if (exercise.Choices != null && exercise.Choices.Any())
                {
                    char choiceLetter = 'A';
                    foreach (var choice in exercise.Choices)
                    {
                        var isCorrectChoice = includeAnswers && exercise.CorrectAnswer == choice;
                        
                        var choiceRun = new Run(new Text($"{choiceLetter}. {choice}"));
                        
                        // Highlight correct answer if needed
                        if (isCorrectChoice)
                        {
                            choiceRun.RunProperties = new RunProperties(
                                new Color { Val = "FF0000" }, // Red color
                                new Bold()
                            );
                        }

                        var choiceParagraph = new Paragraph(choiceRun);
                        
                        // Add indentation for choices
                        choiceParagraph.ParagraphProperties = new ParagraphProperties(
                            new Indentation { Left = "720" } // 0.5 inch indent
                        );
                        
                        body.AppendChild(choiceParagraph);
                        choiceLetter++;
                    }
                }
                else if (includeAnswers && !string.IsNullOrWhiteSpace(exercise.CorrectAnswer))
                {
                    // Add plain text answer for non-multiple-choice
                    var answerParagraph = new Paragraph(
                        new Run(
                            new RunProperties(
                                new Color { Val = "FF0000" },
                                new Bold()
                            ),
                            new Text($"Đáp án: {exercise.CorrectAnswer}")
                        )
                    );
                    body.AppendChild(answerParagraph);
                }

                // Add empty line between questions
                body.AppendChild(new Paragraph(new Run(new Text(""))));
            }
        }
        return memoryStream.ToArray();
    }
}
