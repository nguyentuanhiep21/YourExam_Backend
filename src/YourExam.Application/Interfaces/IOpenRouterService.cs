namespace YourExam.Application.Interfaces;

public interface IOpenRouterService
{
    Task<string> GenerateContentAsync(string prompt, CancellationToken cancellationToken = default);
}
