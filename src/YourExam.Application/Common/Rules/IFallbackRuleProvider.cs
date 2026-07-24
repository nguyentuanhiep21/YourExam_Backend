namespace YourExam.Application.Common.Rules;

public interface IFallbackRuleProvider
{
    /// <summary>
    /// Trả về chuỗi JSON cấu hình VariablesConfig dựa theo môn học và lớp
    /// </summary>
    string GetFallbackVariablesConfig(string subject, int gradeLevel);
}
