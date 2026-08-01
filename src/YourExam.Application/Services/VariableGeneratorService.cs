using System.Text.Json;
using YourExam.Application.Common.Rules;
using YourExam.Application.Interfaces;

namespace YourExam.Application.Services;

public class GenerateFailedException : Exception
{
    public GenerateFailedException(string message) : base(message) { }
}

public class VariableGeneratorService : IVariableGeneratorService
{
    private readonly IFallbackRuleProvider _fallbackRuleProvider;
    private readonly IMathEvaluatorService _mathEvaluator;

    public VariableGeneratorService(IFallbackRuleProvider fallbackRuleProvider, IMathEvaluatorService mathEvaluator)
    {
        _fallbackRuleProvider = fallbackRuleProvider;
        _mathEvaluator = mathEvaluator;
    }

    public Dictionary<string, double> GenerateVariables(string? variablesConfigJson, string subject, int gradeLevel)
    {
        string? configJson = variablesConfigJson;

        // Nếu template không có config hợp lệ, dùng Fallback Rule
        if (string.IsNullOrWhiteSpace(configJson) || configJson == "{}" || configJson == "[]" || configJson == "null")
        {
            configJson = _fallbackRuleProvider.GetFallbackVariablesConfig(subject, gradeLevel);
        }

        // Parse JSON
        var docOptions = new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };
        using var configDoc = JsonDocument.Parse(configJson, docOptions);
        var root = configDoc.RootElement;
        
        var variablesDef = root.TryGetProperty("variables", out var vProp) ? vProp : default;
        var constraintsDef = root.TryGetProperty("constraints", out var cProp) ? cProp : default;

        int maxRetries = 50;
        int currentAttempt = 0;
        var random = new Random();

        while (currentAttempt < maxRetries)
        {
            var currentVars = new Dictionary<string, double>();

            // 1. Sinh các số random
            if (variablesDef.ValueKind == JsonValueKind.Array)
            {
                foreach (var v in variablesDef.EnumerateArray())
                {
                    string name = v.GetProperty("name").GetString()!;
                    int min = v.TryGetProperty("min", out var minProp) ? minProp.GetInt32() : 1;
                    int max = v.TryGetProperty("max", out var maxProp) ? maxProp.GetInt32() : 10;
                    
                    double val = random.Next(min, max + 1);
                    currentVars[name] = val;
                }
            }

            // 2. Kiểm tra constraints
            bool allConstraintsPassed = true;
            if (constraintsDef.ValueKind == JsonValueKind.Array)
            {
                foreach (var constraint in constraintsDef.EnumerateArray())
                {
                    string? cExpr = constraint.GetString();
                    if (!string.IsNullOrWhiteSpace(cExpr))
                    {
                        bool passed = _mathEvaluator.EvaluateConstraint(cExpr, currentVars);
                        if (!passed)
                        {
                            allConstraintsPassed = false;
                            break;
                        }
                    }
                }
            }

            if (allConstraintsPassed)
            {
                return currentVars;
            }

            currentAttempt++;
        }

        throw new GenerateFailedException($"Không thể sinh ra bộ biến số thoả mãn constraints sau {maxRetries} lần thử.");
    }
}
