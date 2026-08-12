using System;
using System.Collections.Generic;
using System.Linq;

namespace YourExam.Application.Services.QuestionGeneration;

public class QuestionGeneratorFactory : IQuestionGeneratorFactory
{
    private readonly IEnumerable<IQuestionGeneratorStrategy> _strategies;
    
    public QuestionGeneratorFactory(IEnumerable<IQuestionGeneratorStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IQuestionGeneratorStrategy GetStrategy(string subject, int gradeLevel, Domain.Enums.ExerciseType exerciseType)
    {
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(subject, gradeLevel, exerciseType));
        if (strategy == null) 
        {
            throw new NotSupportedException($"Chưa có luồng xử lý sinh đề cho Môn {subject} - Lớp {gradeLevel} - Dạng bài {exerciseType}");
        }
        return strategy;
    }
}
