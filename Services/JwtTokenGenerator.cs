using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SteveAPI.Services
{
    /// <summary>
    /// Servicio central para generar tokens JWT.
    /// </summary>
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Genera un token JWT con duración variable.
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <param name="role">Rol del usuario.</param>
        /// <param name="expireMinutes">
        ///     Minutos hasta la expiración (si no se indica, usa el valor de appsettings).
        /// </param>
        public string GenerateToken(string username, string role, int? expireMinutes = null)
        {
            // Parámetros de configuración
            var key      = _configuration["Jwt:Key"]
                           ?? throw new InvalidOperationException("Jwt:Key no configurado.");
            var issuer   = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            // Minutos de expiración (si null, toma el de appsettings)
            int minutes = expireMinutes
                          ?? int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60");

            // Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var keyBytes   = Encoding.UTF8.GetBytes(key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var creds      = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:  issuer,
                audience: audience,
                claims:  claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
