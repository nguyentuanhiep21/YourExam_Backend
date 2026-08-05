namespace YourExam.Application.Interfaces;

public interface ICurrentUserService
{
    /// <summary>
    /// Supabase user ID (claim "sub" from JWT).
    /// Returns null if the request is unauthenticated.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Returns true if the request carries a valid, authenticated JWT.
    /// </summary>
    bool IsAuthenticated { get; }
}
