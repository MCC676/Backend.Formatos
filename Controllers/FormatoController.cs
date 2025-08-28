using AutoMapper;
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
        private readonly IDocumentoParte _documentoParte;
        private readonly IMapper _mapper;

        public FormatoController(IFormatoService formatoService, IDocumentoParte documentoParte, IMapper mapper)
        {
            _formatoService = formatoService;
            _documentoParte = documentoParte;
            _mapper = mapper;
        }

        [HttpPost("generar-word")]
        public async Task<IActionResult> GenerarWord([FromBody] GenerarFormatoDto dto)
        {
            try
            {
                var (bytes, fileName) = await _formatoService.GenerarDocumentoWordAsync(dto);

                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al generar el formato", detalle = ex.Message });
            }
        }

        [HttpPost("generar-excel")]
        public async Task<IActionResult> GenerarExcel([FromBody] GenerarFormatoDto dto)
        {
            try
            {
                var (bytes, fileName) = await _formatoService.GenerarDocumentoExcelAsync(dto);

                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al generar el formato Excel", detalle = ex.Message });
            }
        }


        [HttpGet("historial")]
        public async Task<IEnumerable<FormatoGeneradoDto>> Historial([FromQuery] int? clienteId, [FromQuery] int? agenciaId)
        => await _formatoService.GetHistorialAsync(clienteId, agenciaId);

        [HttpGet("listarDocParte")]
        public async Task<ActionResult<IEnumerable<DocumentoParteDto>>> ListarDocParte()
        {
            var clientes = await _documentoParte.ObtenerDocumentoParteAsync();
            return Ok(_mapper.Map<List<DocumentoParteDto>>(clientes));
        }
    }
}
