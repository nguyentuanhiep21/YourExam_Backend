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
                
                // Determine prefix based on multiple choice or not, and ExerciseType
                bool isMultipleChoice = exercise.Choices != null && exercise.Choices.Any();
                string boldPrefix = $"Bài {i + 1}: ";
                string normalPrefix = "";
                string contentText = exercise.Content;

                if (isMultipleChoice)
                {
                    switch (exercise.ExerciseType)
                    {
                        case 1: // Calculation
                            normalPrefix = "Kết quả phép tính ";
                            contentText = $"{exercise.Content} là";
                            break;
                        case 3: // Comparison
                            contentText = $"{exercise.Content}, dấu phù hợp để điền vào chỗ trống là";
                            break;
                        case 4: // FillInTheBlank
                            contentText = $"{exercise.Content}, số phù hợp để điền vào chỗ trống là";
                            break;
                    }
                }
                else
                {
                    switch (exercise.ExerciseType)
                    {
                        case 1: // Calculation
                            normalPrefix = "Tính kết quả của phép tính ";
                            break;
                        case 3: // Comparison
                            normalPrefix = "Điền dấu phù hợp vào chỗ trống ";
                            break;
                        case 4: // FillInTheBlank
                            normalPrefix = "Điền số phù hợp vào chỗ trống ";
                            break;
                    }
                }

                // Add Question Content
                var questionParagraph = new Paragraph(
                    new Run(
                        new RunProperties(new Bold()),
                        new Text(boldPrefix) { Space = SpaceProcessingModeValues.Preserve }
                    ),
                    new Run(
                        new Text(normalPrefix + contentText) { Space = SpaceProcessingModeValues.Preserve }
                    )
                );
                body.AppendChild(questionParagraph);

                if (!isMultipleChoice)
                {
                    // Bài toán có lời văn (Type = 2) hoặc Đọc hiểu (Type = 9) thường cần nhiều dòng hơn
                    int lineCount = (exercise.ExerciseType == 2 || exercise.ExerciseType == 9) ? 3 : 1;

                    for (int j = 0; j < lineCount; j++)
                    {
                        body.AppendChild(new Paragraph(
                            new ParagraphProperties(
                                new Tabs(
                                    new TabStop() { Val = TabStopValues.Right, Leader = TabStopLeaderCharValues.Dot, Position = 9026 }
                                )
                            ),
                            new Run(new TabChar())
                        ));
                    }
                }

                // Add Choices if it's a multiple choice question
                if (exercise.Choices != null && exercise.Choices.Any())
                {
                    int maxLength = exercise.Choices.Max(c => c?.Length ?? 0);
                    int columns = maxLength < 15 ? 4 : (maxLength < 40 ? 2 : 1);

                    Table table = new Table();
                    TableProperties tblProp = new TableProperties(
                        new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "5000" },
                        new TableBorders(
                            new TopBorder() { Val = BorderValues.None },
                            new BottomBorder() { Val = BorderValues.None },
                            new LeftBorder() { Val = BorderValues.None },
                            new RightBorder() { Val = BorderValues.None },
                            new InsideHorizontalBorder() { Val = BorderValues.None },
                            new InsideVerticalBorder() { Val = BorderValues.None }
                        ),
                        // Add some indentation to the table itself
                        new TableIndentation() { Width = 720, Type = TableWidthUnitValues.Dxa }
                    );
                    table.AppendChild(tblProp);

                    int rows = (int)Math.Ceiling(exercise.Choices.Count / (double)columns);
                    char choiceLetter = 'A';

                    for (int r = 0; r < rows; r++)
                    {
                        TableRow tr = new TableRow();
                        for (int c = 0; c < columns; c++)
                        {
                            int index = r * columns + c;
                            TableCell tc = new TableCell();

                            tc.AppendChild(new TableCellProperties(
                                new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = (5000 / columns).ToString() }
                            ));

                            if (index < exercise.Choices.Count)
                            {
                                var choice = exercise.Choices[index];
                                var isCorrectChoice = includeAnswers && exercise.CorrectAnswer == choice;
                                var choiceRun = new Run(new Text($"{choiceLetter}. {choice}") { Space = SpaceProcessingModeValues.Preserve });

                                if (isCorrectChoice)
                                {
                                    choiceRun.RunProperties = new RunProperties(
                                        new Color { Val = "FF0000" },
                                        new Bold()
                                    );
                                }

                                tc.AppendChild(new Paragraph(choiceRun));
                                choiceLetter++;
                            }
                            else
                            {
                                tc.AppendChild(new Paragraph(new Run(new Text(""))));
                            }
                            tr.AppendChild(tc);
                        }
                        table.AppendChild(tr);
                    }
                    body.AppendChild(table);
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

                // Removed empty paragraph to reduce spacing between questions
            }
        }
        return memoryStream.ToArray();
    }
}
