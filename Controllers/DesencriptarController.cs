using Microsoft.AspNetCore.Mvc;
using SteveAPI.Services;

namespace SteveAPI.Controllers
{
    /// <summary>
    /// Devuelve el texto plano de un mensaje encriptado.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DesencriptarController : ControllerBase
    {
        private readonly DesencriptarService _svc;

        public DesencriptarController(DesencriptarService svc) => _svc = svc;

        // ------------------------------------------------------------------
        // GET: api/Desencriptar/5
        // Devuelve el texto desencriptado del registro con Id indicado.
        // ------------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            var plain = await _svc.DesencriptarPorIdAsync(id);
            return plain is null ? NotFound() : Ok(plain);
        }
    }
}
