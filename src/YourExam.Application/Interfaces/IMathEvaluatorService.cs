namespace YourExam.Application.Interfaces;

public interface IMathEvaluatorService
{
    /// <summary>
    /// Đánh giá biểu thức toán học trả về kết quả số học (vd: "x + y").
    /// </summary>
    double EvaluateMathExpression(string expression, Dictionary<string, double> variables);

    /// <summary>
    /// Đánh giá biểu thức logic trả về boolean (vd: "x + y <= 80").
    /// </summary>
    bool EvaluateConstraint(string constraintExpression, Dictionary<string, double> variables);
}
