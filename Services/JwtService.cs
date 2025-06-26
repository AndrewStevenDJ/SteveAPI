using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expireMinutes;

    public JwtService(IConfiguration config)
    {
        // Usa el operador "!" para indicar que estos valores no son nulos
        _key = config["Jwt:Key"] ?? throw new ArgumentNullException(nameof(config), "Jwt:Key is missing");
        _issuer = config["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(config), "Jwt:Issuer is missing");
        _audience = config["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(config), "Jwt:Audience is missing");

        var expireStr = config["Jwt:ExpireMinutes"] ?? throw new ArgumentNullException(nameof(config), "Jwt:ExpireMinutes is missing");
        if (!int.TryParse(expireStr, out _expireMinutes))
        {
            throw new ArgumentException("Jwt:ExpireMinutes is not a valid integer");
        }
    }

    public string GenerateToken(string username)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            // Agrega más claims si quieres
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_expireMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
