using TravelDNA.Core.Models;

namespace TravelDNA.Core.Interfaces;

public sealed record TokenPair(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt, DateTime RefreshTokenExpiresAt);

public interface ITokenService
{
    TokenPair CreateTokenPair(User user);
}