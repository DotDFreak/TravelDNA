namespace TravelDNA.Core.Interfaces;

public sealed record GoogleUserInfo(string SubjectId, string Email, string? DisplayName);

public interface IGoogleTokenVerifier
{
    Task<GoogleUserInfo?> VerifyTokenAsync(string token, CancellationToken cancellationToken = default);
}