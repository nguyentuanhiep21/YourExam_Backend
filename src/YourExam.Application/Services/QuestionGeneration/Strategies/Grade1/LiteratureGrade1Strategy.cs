using System.Text.Json.Nodes;
using YourExam.Application.Features.Exercises.Commands.GenerateExercise;
using YourExam.Application.Interfaces;
using YourExam.Domain.Entities;

namespace YourExam.Application.Services.QuestionGeneration.Strategies.Grade1;

/// <summary>
/// Xử lý sinh bài tập môn Tiếng Việt Lớp 1.
/// Bốc ngẫu nhiên từ LiteratureDictionaryService (O(1), không dùng DB).
/// Template là dummy object mang ExerciseType, không cần VariablesConfig.
/// </summary>
public class LiteratureGrade1Strategy : IQuestionGeneratorStrategy
{
    private static readonly Random _random = new();
    private readonly ILiteratureDictionaryService _dictService;

    public LiteratureGrade1Strategy(ILiteratureDictionaryService dictService)
    {
        _dictService = dictService;
    }

    public bool CanHandle(string subject, int gradeLevel, Domain.Enums.ExerciseType exerciseType)
    {
        return subject.Equals("tiengviet", StringComparison.OrdinalIgnoreCase) && gradeLevel == 1;
    }

    public Task<GeneratedExerciseDto> GenerateAsync(QuestionTemplate template, Domain.Enums.QuestionFormat format)
    {
        var record = _dictService.GetRandomRecord(template.ExerciseType);
        if (record == null)
            throw new InvalidOperationException($"Không có dữ liệu Dictionary cho dạng bài {template.ExerciseType}.");

        return Task.FromResult(template.ExerciseType switch
        {
            Domain.Enums.ExerciseType.Phonetics                => GeneratePhonetics(record, template.Id, format),
            Domain.Enums.ExerciseType.Spelling                 => GenerateSpelling(record, template.Id, format),
            Domain.Enums.ExerciseType.FillInBlank              => GenerateFillBlank(record, template.Id),
            Domain.Enums.ExerciseType.WordOrder                => GenerateWordOrder(record, template.Id, format),
            Domain.Enums.ExerciseType.OddOneOut                => GenerateOddOneOut(record, template.Id),
            Domain.Enums.ExerciseType.Reading                  => GenerateReading(record, template.Id),
            _ => throw new NotSupportedException($"Dạng bài {template.ExerciseType} chưa được hỗ trợ.")
        });
    }

    // ─── Dạng 5: Nhận biết vần (Phonetics) ─────────────────────────────────
    private GeneratedExerciseDto GeneratePhonetics(JsonObject record, int templateId, Domain.Enums.QuestionFormat format)
    {
        var target    = record["Target_Van"]?.GetValue<string>() ?? "";
        string templateStr = null;
        if (format == Domain.Enums.QuestionFormat.Essay)
            templateStr = record["Template_TL"]?.GetValue<string>();
        
        if (string.IsNullOrEmpty(templateStr))
            templateStr = record["Template"]?.GetValue<string>();
            
        var template = templateStr?.Replace("[Target]", target) ?? "";
        var corrects  = GetArray(record, "Correct_Words");
        var distractors = GetArray(record, "Distractor_Words");

        var correctAnswer = PickRandom(corrects, 1).First();
        var choices = new List<string> { correctAnswer };
        choices.AddRange(PickRandom(distractors, 3));
        Shuffle(choices);

        return new GeneratedExerciseDto
        {
            TemplateId = templateId,
            Content = template,
            CorrectAnswer = correctAnswer,
            Choices = choices
        };
    }

    // ─── Dạng 6: Quy tắc Chính tả (Spelling) ───────────────────────────────
    private GeneratedExerciseDto GenerateSpelling(JsonObject record, int templateId, Domain.Enums.QuestionFormat format)
    {
        var tail     = record["Tail"]?.GetValue<string>() ?? "";
        string templateStr = null;
        if (format == Domain.Enums.QuestionFormat.Essay)
            templateStr = record["Template_TL"]?.GetValue<string>();
        
        if (string.IsNullOrEmpty(templateStr))
            templateStr = record["Template"]?.GetValue<string>();
            
        var template = templateStr?.Replace("[Tail]", tail) ?? "";
        var corrects  = GetArray(record, "Correct");
        var distractors = GetArray(record, "Distractors");

        var correctAnswer = corrects.FirstOrDefault() ?? "";
        var choices = new List<string> { correctAnswer };
        choices.AddRange(distractors.Where(d => d != correctAnswer));
        Shuffle(choices);

        return new GeneratedExerciseDto
        {
            TemplateId = templateId,
            Content = template,
            CorrectAnswer = correctAnswer,
            Choices = choices
        };
    }

    // ─── Dạng 7: Sắp xếp từ thành câu có nghĩa (WordOrder) ─────────────────
    private GeneratedExerciseDto GenerateWordOrder(JsonObject record, int templateId, Domain.Enums.QuestionFormat format)
    {
        var correctSentence = record["Correct_Sentence"]?.GetValue<string>() ?? "";
        var shuffled = GetArray(record, "Shuffled_Words");
        var wrongOrders = GetArray(record, "Distractors_WrongOrder");

        var content = (format == Domain.Enums.QuestionFormat.Essay 
            ? "Sắp xếp các từ sau thành câu có nghĩa: " 
            : "Chọn câu có nghĩa được tạo nên bởi các từ sau: ") + string.Join(" / ", shuffled);

        var choices = new List<string> { correctSentence };
        choices.AddRange(PickRandom(wrongOrders, 3));
        Shuffle(choices);

        return new GeneratedExerciseDto
        {
            TemplateId = templateId,
            Content = content,
            CorrectAnswer = correctSentence,
            Choices = choices
        };
    }

    // ─── Dạng 8: Tìm từ khác loại (OddOneOut) ──────────────────────────────
    private GeneratedExerciseDto GenerateOddOneOut(JsonObject record, int templateId)
    {
        var category  = record["Category"]?.GetValue<string>() ?? "";
        var group     = GetArray(record, "Correct_Group");
        var outliers  = GetArray(record, "Distractor_Group");

        // Bốc 3 từ đúng + 1 từ khác loại → Shuffle → Hỏi từ nào không cùng nhóm
        var threeCorrect = PickRandom(group, 3);
        var oneOutlier   = PickRandom(outliers, 1).First();

        var choices = new List<string>(threeCorrect) { oneOutlier };
        Shuffle(choices);

        var content = $"Tìm từ không cùng nhóm với các từ còn lại (Chủ đề: {category}):";

        return new GeneratedExerciseDto
        {
            TemplateId = templateId,
            Content = content,
            CorrectAnswer = oneOutlier,
            Choices = choices
        };
    }

    // ─── Dạng 9: Đọc hiểu văn bản ngắn (Reading) ───────────────────────────
    private GeneratedExerciseDto GenerateReading(JsonObject record, int templateId)
    {
        var passage   = record["Passage"]?.GetValue<string>() ?? "";
        var question  = record["Question"]?.GetValue<string>() ?? "";
        var corrects  = GetArray(record, "Correct_Answers");
        var distractors = GetArray(record, "Distractors");

        var correctAnswer = corrects.FirstOrDefault() ?? "";
        var content = $"{passage}\n\n{question}";

        var choices = new List<string> { correctAnswer };
        choices.AddRange(PickRandom(distractors, 3));
        Shuffle(choices);

        return new GeneratedExerciseDto
        {
            TemplateId = templateId,
            Content = content,
            CorrectAnswer = correctAnswer,
            Choices = choices
        };
    }

    // ─── Dạng 10: Điền/Chọn từ (FillInBlank) ───────────────────────────────
    private GeneratedExerciseDto GenerateFillBlank(JsonObject record, int templateId)
    {
        var content  = record["Sentence_Template"]?.GetValue<string>() ?? "";
        var corrects  = GetArray(record, "Correct_Words");
        var distractors = GetArray(record, "Distractors");

        var correctAnswer = PickRandom(corrects, 1).First();
        var choices = new List<string> { correctAnswer };
        choices.AddRange(PickRandom(distractors, 3));
        Shuffle(choices);

        return new GeneratedExerciseDto
        {
            TemplateId = templateId,
            Content = content,
            CorrectAnswer = correctAnswer,
            Choices = choices
        };
    }
    
    // ─── Helpers ─────────────────────────────────────────────────────────────
    private static List<string> GetArray(JsonObject record, string key)
    {
        var node = record[key];
        if (node is not JsonArray arr) return new();
        return arr
            .Select(x => x?.GetValue<string>())
            .Where(x => x != null)
            .Cast<string>()
            .ToList();
    }

    private static List<string> PickRandom(List<string> source, int count)
    {
        if (source.Count <= count) return source.ToList();
        return source.OrderBy(_ => _random.Next()).Take(count).ToList();
    }

    private static void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
