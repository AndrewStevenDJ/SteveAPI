using GeometriaAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace GeometriaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CifradoController : ControllerBase
    {
        private readonly CesarService _cesarService;
        private const int DESPLAZAMIENTO = 3; // Valor fijo

        public CifradoController()
        {
            _cesarService = new CesarService();
        }

        [HttpPost("encriptar")]
        public IActionResult Encriptar([FromBody] CifradoRequest request)
        {
            var resultado = _cesarService.Encriptar(request.Mensaje, DESPLAZAMIENTO);
            return Ok(new { mensajeEncriptado = resultado });
        }

        [HttpPost("desencriptar")]
        public IActionResult Desencriptar([FromBody] CifradoRequest request)
        {
            var resultado = _cesarService.Desencriptar(request.Mensaje, DESPLAZAMIENTO);
            return Ok(new { mensajeDesencriptado = resultado });
        }
    }

    public class CifradoRequest
    {
        public string Mensaje { get; set; } = string.Empty;
        // Desplazamiento ya no se usa aquí
    }
}
