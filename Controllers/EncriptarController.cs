using Microsoft.AspNetCore.Authorization;  // <-- Agregado
using Microsoft.AspNetCore.Mvc;
using SteveAPI.Models;
using SteveAPI.Services;

namespace SteveAPI.Controllers
{
    /// <summary>
    /// CRUD de mensajes encriptados.
    /// </summary>
    [Authorize]  // <-- Agregado para proteger todo el controlador
    [ApiController]
    [Route("api/[controller]")]
    public class EncriptarController : ControllerBase
    {
        private readonly EncriptarService _svc;

        public EncriptarController(EncriptarService svc) => _svc = svc;

        [HttpGet]
        public async Task<ActionResult<List<Encriptar>>> GetAll() =>
            Ok(await _svc.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Encriptar>> GetById(int id)
        {
            var item = await _svc.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Encriptar>> Create([FromBody] string textoPlano)
        {
            var creado = await _svc.CreateAsync(textoPlano);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] string textoPlano)
        {
            var existente = await _svc.GetByIdAsync(id);
            if (existente is null) return NotFound();

            existente.TextoCifrado = (await _svc.CreateAsync(textoPlano)).TextoCifrado;
            await _svc.ReplaceAsync(existente);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            await _svc.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
