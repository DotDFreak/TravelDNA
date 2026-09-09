using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using TravelDNA.Core.Interfaces;

namespace TravelDNA.Infrastructure.Services;

public sealed class GoogleTokenVerifier(IConfiguration configuration) : IGoogleTokenVerifier
{
    public async Task<GoogleUserInfo?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(idToken))
        {
            throw new ArgumentException("A Google ID token is required.", nameof(idToken));
        }

        var clientID = configuration["Google:ClientId"];
        if(string.IsNullOrWhiteSpace(clientID))
        {
            throw new InvalidOperationException("Google:ClientId is not configured.");
        }

        var validationSettings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [clientID]
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);
        return new GoogleUserInfo(payload.Subject, payload.Email, payload.Name);
    }
}