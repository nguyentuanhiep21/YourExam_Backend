using System;

namespace YourExam.Application.Common.Rules;

public class FallbackRuleProvider : IFallbackRuleProvider
{
    public string GetFallbackVariablesConfig(string subject, int gradeLevel)
    {
        // 1. Môn Toán
        if (subject.Equals("Toán", StringComparison.OrdinalIgnoreCase) || subject.Equals("Math", StringComparison.OrdinalIgnoreCase))
        {
            if (gradeLevel == 1)
            {
                // Lớp 1: Số lượng nhỏ (ví dụ 1->10), tổng <= 10 hoặc <= 20
                return @"{
                    ""variables"": [
                        { ""name"": ""x"", ""min"": 1, ""max"": 10 },
                        { ""name"": ""y"", ""min"": 1, ""max"": 10 }
                    ],
                    ""constraints"": [
                        ""x + y <= 10""
                    ]
                }";
            }
            else if (gradeLevel == 2)
            {
                // Lớp 2: Có thể lớn hơn, vd: <= 100
                return @"{
                    ""variables"": [
                        { ""name"": ""x"", ""min"": 10, ""max"": 50 },
                        { ""name"": ""y"", ""min"": 10, ""max"": 50 }
                    ],
                    ""constraints"": [
                        ""x + y <= 100""
                    ]
                }";
            }
        }

        // Rule mặc định chung chung (nếu không map trúng)
        return @"{
            ""variables"": [
                { ""name"": ""x"", ""min"": 1, ""max"": 10 }
            ]
        }";
    }
}
