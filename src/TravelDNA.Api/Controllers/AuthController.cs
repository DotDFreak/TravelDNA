using Microsoft.AspNetCore.Mvc;
using TravelDNA.Core.Interfaces;
using TravelDNA.Core.Models;

namespace TravelDNA.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IGoogleTokenVerifier googleTokenVerifier, 
    IUserRepository userRepository, 
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("google")]
    public async Task<ActionResult<TokenResponse>> GoogleLoginAsync(
        GoogleLoginRequest request,
        CancellationToken cancellationToken)
    {
        GoogleUserInfo? googleUserInfo;
        try
        {
            googleUserInfo = await googleTokenVerifier.VerifyTokenAsync(request.IdToken, cancellationToken);
        }
        catch (ArgumentException)
        {
            return BadRequest("A Google ID token is required.");
        }
        catch (InvalidOperationException)
        {
            return Problem("Google sign-in is not configured for this environment.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
        catch
        {
            return Unauthorized("The Google ID token is invalid or expired.");
        }

        if (googleUserInfo is null)
        {
            return Unauthorized("The Google ID token is invalid or expired.");
        }

        var user = await userRepository.FindByGoogleSubjectIdAsync(googleUserInfo.SubjectId, cancellationToken);
        if(user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = googleUserInfo.Email,
                DisplayName = googleUserInfo.DisplayName,
                GoogleSubjectId = googleUserInfo.SubjectId,
                AuthProvider = "Google",
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            user.Email = googleUserInfo.Email;
            user.DisplayName = googleUserInfo.DisplayName;
            user.UpdatedAtUtc = DateTime.UtcNow;
        }

        await userRepository.SaveChangesAsync(cancellationToken);

        var tokens = tokenService.CreateTokenPair(user);

        return Ok(new TokenResponse(
            AccessToken: tokens.AccessToken,
            RefreshToken: tokens.RefreshToken,
            AccessTokenExpiresAtUtc: tokens.AccessTokenExpiresAt));
    }

public sealed record GoogleLoginRequest(string IdToken);

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc);
}