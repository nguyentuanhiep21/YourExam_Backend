using MediatR;
using YourExam.Application.DTOs;

namespace YourExam.Application.Features.QuestionTemplates.Queries.GetQuestionTemplateById;

public class GetQuestionTemplateByIdQuery : IRequest<QuestionTemplateDto?>
{
    public int Id { get; set; }

    public GetQuestionTemplateByIdQuery(int id)
    {
        Id = id;
    }
}
