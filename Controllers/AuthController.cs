using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Aquí validas el usuario y contraseña. Ejemplo simple:
        if (request.Username == "admin" && request.Password == "1234")
        {
            var token = _jwtService.GenerateToken(request.Username);
            return Ok(new { Token = token });
        }
        return Unauthorized();
    }
}
