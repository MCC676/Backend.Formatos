using BackendFormatos.Models.ContentResponse;
using BackendFormatos.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendFormatos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormatoController : ControllerBase
    {
        private readonly IFormatoService _formatoService;

        public FormatoController(IFormatoService formatoService)
        {
            _formatoService = formatoService;
        }

        [HttpPost("generar-word")]
        public async Task<IActionResult> GenerarWord([FromBody] GenerarFormatoDto dto)
        {
            try
            {
                var wordBytes = await _formatoService.GenerarDocumentoWordAsync(dto);

                return File(wordBytes,
                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                            $"formato-{dto.FormatoSeleccionado}-{DateTime.Now:yyyyMMddHHmmss}.docx");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al generar el formato", detalle = ex.Message });
            }
        }
    }
}
