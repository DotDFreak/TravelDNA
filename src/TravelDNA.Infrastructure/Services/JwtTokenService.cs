using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TravelDNA.Core.Interfaces;
using TravelDNA.Core.Models;

namespace TravelDNA.Infrastructure.Services;

public sealed class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public TokenPair CreateTokenPair(User user)
    {
        var secret = configuration["Jwt:Secret"];
        if(string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Secret is not configured or is too short. It must be at least 32 characters long.");
        }

        var issuer = configuration["Jwt:Issuer"] ?? "TravelDNA";
        var audience = configuration["Jwt:Audience"] ?? "TravelDNA.Web";
        var accessTokenMinutes = configuration.GetValue("Jwt:AccessTokenMinutes", 15);
        var refreshTokenDays = configuration.GetValue("Jwt:RefreshTokenDays", 30);
        var now = DateTime.UtcNow;
        var accessTokenExpiresAt = now.AddMinutes(accessTokenMinutes);
        var refreshTokenExpiresAt = now.AddDays(refreshTokenDays);

        var claims = new []
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("auth_provider", user.AuthProvider)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: accessTokenExpiresAt,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return new TokenPair(accessToken, refreshToken, accessTokenExpiresAt, refreshTokenExpiresAt);
    }
}