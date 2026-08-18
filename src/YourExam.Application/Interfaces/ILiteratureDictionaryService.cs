using System.Text.Json.Nodes;
using YourExam.Domain.Enums;

namespace YourExam.Application.Interfaces;

public interface ILiteratureDictionaryService
{
    /// <summary>
    /// Lấy ngẫu nhiên một bản ghi dựa trên loại bài tập
    /// </summary>
    JsonObject? GetRandomRecord(ExerciseType exerciseType);
}
