using System.Text.Json;
using System.Text.RegularExpressions;
using YourExam.Application.Interfaces;

namespace YourExam.Infrastructure.Services;

public class TextVariableGeneratorService : ITextVariableGeneratorService
{
    private readonly Dictionary<string, List<string>> _dictionary;

    public TextVariableGeneratorService()
    {
        // Load global dictionary from file
        string dictionaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Common", "Rules", "Dictionaries", "Entities.json");
        if (File.Exists(dictionaryPath))
        {
            string jsonContent = File.ReadAllText(dictionaryPath);
            _dictionary = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonContent) ?? new();
        }
        else
        {
            _dictionary = new();
        }
    }

    public string ReplaceTextVariables(string contentTemplate)
    {
        if (string.IsNullOrWhiteSpace(contentTemplate))
            return contentTemplate;

        // Tracks chosen values for variables in the current execution to ensure consistency and uniqueness
        // Example: NhanVat_1 -> "An", NhanVat_2 -> "Bình"
        var chosenValuesMap = new Dictionary<string, string>();
        
        // Tracks used values per base key to prevent duplicates
        // Example: NhanVat -> ["An", "Bình"]
        var usedValuesPerBaseKey = new Dictionary<string, HashSet<string>>();

        var random = new Random();

        // Regex to match [Key] or [Key_Suffix] e.g., [NhanVat], [NhanVat_1], [NguoiCho_2]
        string pattern = @"\[([a-zA-Z0-9_]+)\]";
        
        string result = Regex.Replace(contentTemplate, pattern, match =>
        {
            string fullVariableKey = match.Groups[1].Value; // e.g., NhanVat_1
            
            // If we already chose a value for this exact variable in this string, reuse it
            if (chosenValuesMap.TryGetValue(fullVariableKey, out string? chosenValue))
            {
                return chosenValue;
            }

            // Extract base key by removing suffix if exists (e.g., NhanVat_1 -> NhanVat)
            string baseKey = Regex.Replace(fullVariableKey, @"_\d+$", "");

            if (_dictionary.TryGetValue(baseKey, out var availableValues) && availableValues.Any())
            {
                if (!usedValuesPerBaseKey.ContainsKey(baseKey))
                {
                    usedValuesPerBaseKey[baseKey] = new HashSet<string>();
                }
                
                var usedValues = usedValuesPerBaseKey[baseKey];
                
                // Get available values that haven't been used yet
                var unusedValues = availableValues.Where(v => !usedValues.Contains(v)).ToList();
                
                // If we ran out of unique values, reset the pool for this base key
                if (!unusedValues.Any())
                {
                    usedValues.Clear();
                    unusedValues = availableValues.ToList();
                }

                // Pick a random value from the available unused values
                string pickedValue = unusedValues[random.Next(unusedValues.Count)];
                
                // Record the choice
                usedValues.Add(pickedValue);
                chosenValuesMap[fullVariableKey] = pickedValue;

                return pickedValue;
            }

            // Fallback: If not found in dictionary, keep the original placeholder
            return match.Value;
        });

        return result;
    }
}
