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
        double finalResult = Convert.ToDouble(result);
        
        if (double.IsInfinity(finalResult) || double.IsNaN(finalResult))
        {
            throw new DivideByZeroException($"Biểu thức {expression} sinh ra lỗi Toán học (Infinity/NaN).");
        }

        return finalResult;
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

    public string EvaluateStringExpression(string expression, Dictionary<string, double> variables)
    {
        // Handle ternary operator for NCalc if it doesn't support it natively
        // Convert: A > B ? '>' : (A < B ? '<' : '=')
        // To: if(A > B, '>', if(A < B, '<', '='))
        string processedExpression = expression;
        if (processedExpression.Contains("?") && processedExpression.Contains(":"))
        {
            // A simple naive replacement for the exact pattern we used in our JSON templates
            // This replaces the outer ternary
            int firstQuestionMark = processedExpression.IndexOf('?');
            int firstColon = processedExpression.IndexOf(':', firstQuestionMark); // wait, nested colons
            
            // To be safe, if the string has our exact comparison pattern, let's just evaluate it safely
            // However, modern NCalc often supports ternary. We will just pass it to NCalc.
            // If it fails, we will catch and provide a helpful message to change the DB to use if().
        }

        try
        {
            var e = new Expression(processedExpression);
            foreach (var kvp in variables)
            {
                e.Parameters[kvp.Key] = kvp.Value;
            }
            return e.Evaluate()?.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            // Fallback parsing just in case NCalc fails on ? :
            if (expression.Contains("?") && expression.Contains("'<Id'")) // just a dummy check
            {
                // this is getting complex, let's just throw
            }
            
            // Let's do a smart fallback for our specific Comparison AnswerFormula
            // "X ? '>' : (Y ? '<' : '=')"
            if (expression.Contains("'>'") && expression.Contains("'<['") == false)
            {
                 // We can evaluate the two sides of the > operator for the first condition
                 var firstCondStr = expression.Substring(0, expression.IndexOf('?')).Trim(); // e.g. "a > (b + c)"
                 try {
                     var eCond = new Expression(firstCondStr);
                     foreach(var kvp in variables) eCond.Parameters[kvp.Key] = kvp.Value;
                     bool isGreater = Convert.ToBoolean(eCond.Evaluate());
                     if (isGreater) return ">";
                     
                     var secondCondStrStart = expression.IndexOf('(', expression.IndexOf(':'));
                     var secondCondStrEnd = expression.IndexOf('?', secondCondStrStart);
                     if (secondCondStrStart != -1 && secondCondStrEnd != -1) {
                         var secondCondStr = expression.Substring(secondCondStrStart + 1, secondCondStrEnd - secondCondStrStart - 1).Trim();
                         var eCond2 = new Expression(secondCondStr);
                         foreach(var kvp in variables) eCond2.Parameters[kvp.Key] = kvp.Value;
                         bool isLess = Convert.ToBoolean(eCond2.Evaluate());
                         if (isLess) return "<";
                     }
                     return "=";
                 } catch { }
            }

            throw new InvalidOperationException($"Lỗi phân tích biểu thức NCalc: {ex.Message}. (Gợi ý: Nếu dùng NCalc không hỗ trợ toán tử 3 ngôi '?', hãy đổi công thức trong DB thành dạng if(điều kiện, đúng, sai)).", ex);
        }
    }
}
