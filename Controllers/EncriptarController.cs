using Microsoft.AspNetCore.Mvc;
using SteveAPI.Models;
using SteveAPI.Services;

namespace SteveAPI.Controllers
{
    /// <summary>
    /// CRUD de mensajes encriptados.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EncriptarController : ControllerBase
    {
        private readonly EncriptarService _svc;

        public EncriptarController(EncriptarService svc) => _svc = svc;

        // ------------------------------------------------------------------
        // GET: api/Encriptar
        // Devuelve todos los registros encriptados (solo texto cifrado).
        // ------------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<List<Encriptar>>> GetAll() =>
            Ok(await _svc.GetAllAsync());

        // ------------------------------------------------------------------
        // GET: api/Encriptar/5
        // Devuelve un registro por Id (texto cifrado).
        // ------------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Encriptar>> GetById(int id)
        {
            var item = await _svc.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        // ------------------------------------------------------------------
        // POST: api/Encriptar
        // Encripta el texto recibido y lo guarda.
        // Body (plain JSON string):  "Hola Steve"
        // ------------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<Encriptar>> Create([FromBody] string textoPlano)
        {
            var creado = await _svc.CreateAsync(textoPlano);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        // ------------------------------------------------------------------
        // PUT: api/Encriptar/5
        // Actualiza un registro reemplazando su texto cifrado
        // con la nueva versión encriptada de 'textoPlano'.
        // ------------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] string textoPlano)
        {
            var existente = await _svc.GetByIdAsync(id);
            if (existente is null) return NotFound();

            // Actualiza el texto cifrado
            existente.TextoCifrado = (await _svc.CreateAsync(textoPlano)).TextoCifrado;

            // Guarda los cambios (CreateAsync ya hace SaveChanges,
            // así que llamamos UpdateAsync para no duplicar registro)
            await _svc.ReplaceAsync(existente);
            return NoContent();
        }

        // ------------------------------------------------------------------
        // DELETE: api/Encriptar/5
        // Elimina un registro.
        // ------------------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            await _svc.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
