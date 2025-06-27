using Microsoft.AspNetCore.Authorization;  // <-- Agregado
using Microsoft.AspNetCore.Mvc;
using SteveAPI.Services;

namespace SteveAPI.Controllers
{
    /// <summary>
    /// Devuelve el texto plano de un mensaje encriptado.
    /// </summary>
    [Authorize]  // <-- Agregado para proteger todo el controlador
    [ApiController]
    [Route("api/[controller]")]
    public class DesencriptarController : ControllerBase
    {
        private readonly DesencriptarService _svc;

        public DesencriptarController(DesencriptarService svc) => _svc = svc;

        [HttpGet("{id:int}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            var plain = await _svc.DesencriptarPorIdAsync(id);
            return plain is null ? NotFound() : Ok(plain);
        }
    }
}
