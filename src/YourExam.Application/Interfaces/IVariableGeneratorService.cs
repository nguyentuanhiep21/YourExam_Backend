namespace YourExam.Application.Interfaces;

public interface IVariableGeneratorService
{
    /// <summary>
    /// Sinh ra Dictionary chứa tên biến và giá trị. Sẽ retry tối đa 50 lần để thoả mãn constraints.
    /// Ném lỗi GenerateFailedException nếu thất bại.
    /// </summary>
    Dictionary<string, double> GenerateVariables(string? variablesConfigJson, string subject, int gradeLevel);
}
