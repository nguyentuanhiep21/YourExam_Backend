using MediatR;

namespace YourExam.Application.Features.QuestionTemplates.Commands.AutoGenerateTemplates;

public class AutoGenerateTemplatesCommand : IRequest<int>
{
    public string Subject { get; set; } = "Toán";
    public int GradeLevel { get; set; }
    public int QuestionType { get; set; }
    
    public int EasyQuantity { get; set; }
    public int MediumQuantity { get; set; }
    public int HardQuantity { get; set; }
}
