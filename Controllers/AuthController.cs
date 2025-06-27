using Microsoft.AspNetCore.Mvc;
using SteveAPI.DTOs;
using SteveAPI.Models;
using SteveAPI.Services;

namespace SteveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // Usuarios de prueba en memoria
    private static readonly List<User> _users = new()
    {
        new User { Username = "admin", Password = "admin123", Role = "Admin" },
        new User { Username = "user",  Password = "user123",  Role = "User"  }
    };

    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthController(JwtTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var user = _users.FirstOrDefault(u =>
            u.Username == req.Username && u.Password == req.Password);

        if (user is null) return Unauthorized("Credenciales inválidas");

        // ⚠️ Token de 10 minutos para este ejemplo
        var token = _tokenGenerator.GenerateToken(user.Username, user.Role, 10);

        return Ok(new { token });
    }
}
