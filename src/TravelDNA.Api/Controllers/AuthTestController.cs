using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelDNA.Api.Controllers;

[ApiController]
[Route("api/auth-test")]
public sealed class AuthTestController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub"),
            Email = User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue("email"),
            Message = "Authentication works."
        });
    }
}