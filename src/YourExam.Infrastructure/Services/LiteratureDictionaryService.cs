using System.Text.Json.Nodes;
using System.Text.Json;
using YourExam.Domain.Enums;
using YourExam.Application.Interfaces;

namespace YourExam.Infrastructure.Services;

/// <summary>
/// Load toàn bộ Dictionaries JSON vào RAM một lần lúc startup (Singleton).
/// Truy xuất O(1) không có I/O.
/// Khi scale sau này: chỉ cần thay thế class này bằng một implementation
/// đọc từ Supabase, không cần đụng vào Strategy hay CommandHandler.
/// </summary>
public class LiteratureDictionaryService : ILiteratureDictionaryService
{
    private static readonly Random _random = new();
    private readonly Dictionary<ExerciseType, List<JsonObject>> _cache = new();

    public LiteratureDictionaryService()
    {
        LoadAll();
    }

    public JsonObject? GetRandomRecord(ExerciseType exerciseType)
    {
        if (!_cache.TryGetValue(exerciseType, out var records) || records.Count == 0)
            return null;

        return records[_random.Next(records.Count)];
    }

    private void LoadAll()
    {
        // Tìm thư mục Resources/Literature tương đối từ assembly
        var baseDir = AppContext.BaseDirectory;
        var dictPath = FindDictionaryPath(baseDir);

        if (dictPath == null) return;

        var fileMap = new Dictionary<ExerciseType, string>
        {
            { ExerciseType.Phonetics,                Path.Combine(dictPath, "Dict_Phonetics.json") },
            { ExerciseType.Spelling,                 Path.Combine(dictPath, "Dict_Spelling_Rules.json") },
            { ExerciseType.FillInBlank,              Path.Combine(dictPath, "Dict_Fill_Blank.json") },
            { ExerciseType.WordOrder,                Path.Combine(dictPath, "Dict_Word_Order.json") },
            { ExerciseType.OddOneOut,                Path.Combine(dictPath, "Dict_Odd_One_Out.json") },
            { ExerciseType.Reading,                  Path.Combine(dictPath, "Dict_Reading_Mini.json") },
        };

        foreach (var (type, filePath) in fileMap)
        {
            if (!File.Exists(filePath)) continue;

            var json = File.ReadAllText(filePath);
            var array = JsonNode.Parse(json)?.AsArray();
            if (array == null) continue;

            var records = new List<JsonObject>();
            foreach (var item in array)
            {
                if (item is JsonObject obj)
                    records.Add(obj);
            }
            _cache[type] = records;
        }
    }

    /// <summary>
    /// Tìm đường dẫn thư mục Resources/Literature bằng cách leo lên thư mục cha.
    /// Phù hợp cả khi chạy local (dotnet run) lẫn deploy (publish).
    /// </summary>
    private static string? FindDictionaryPath(string startDir)
    {
        // Thứ tự tìm:
        // 1. Publish path: bin/Release/net8.0/Resources/Literature
        var direct = Path.Combine(startDir, "Resources", "Literature");
        if (Directory.Exists(direct)) return direct;

        // 2. Source path: leo lên từ bin/ để tìm src/
        var dir = new DirectoryInfo(startDir);
        for (int i = 0; i < 8; i++)
        {
            if (dir == null) break;
            var candidate = Path.Combine(dir.FullName, "src", "YourExam.Infrastructure", "Resources", "Literature");
            if (Directory.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }

        return null;
    }
}
