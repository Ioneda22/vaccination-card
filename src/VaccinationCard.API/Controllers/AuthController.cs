using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccinationCard.API.Contracts;
using VaccinationCard.Application.Common.Interfaces;

namespace VaccinationCard.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public class AuthController(IJwtTokenGenerator jwtTokenGenerator) : ControllerBase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.UserName != "admin" || request.Password != "admin")
        {
            return Unauthorized(new { title = "Credenciais inválidas." });
        }

        string token = _jwtTokenGenerator.GenerateToken(
            userId: Guid.NewGuid().ToString(),
            userName: request.UserName);

        return Ok(new { accessToken = token, tokenType = "Bearer" });
    }
}
