using Microsoft.EntityFrameworkCore;
using YourExam.Domain.Entities;

namespace YourExam.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<QuestionTemplate> QuestionTemplates { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
