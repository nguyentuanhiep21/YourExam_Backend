using NCalc;
using YourExam.Application.Interfaces;

namespace YourExam.Application.Services;

public class MathEvaluatorService : IMathEvaluatorService
{
    public double EvaluateMathExpression(string expression, Dictionary<string, double> variables)
    {
        var e = new Expression(expression);
        
        foreach (var kvp in variables)
        {
            e.Parameters[kvp.Key] = kvp.Value;
        }

        var result = e.Evaluate();
        return Convert.ToDouble(result);
    }

    public bool EvaluateConstraint(string constraintExpression, Dictionary<string, double> variables)
    {
        var e = new Expression(constraintExpression);
        
        foreach (var kvp in variables)
        {
            e.Parameters[kvp.Key] = kvp.Value;
        }

        var result = e.Evaluate();
        return Convert.ToBoolean(result);
    }
}
