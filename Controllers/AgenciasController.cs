using AutoMapper;
using BackendFormatos.Data;
using BackendFormatos.Models.ContentResponse;
using BackendFormatos.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendFormatos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgenciasController : _BaseController
    {
        private readonly IAgenciaService _service;
        private readonly IMapper _mapper;

        public AgenciasController(DbFormatoContext context, IAgenciaService service, IMapper mapper)
        : base(context)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<AgenciaDto>>> Listar()
        {
            var agencias = await _service.ObtenerAgenciasAsync();
            return Ok(_mapper.Map<List<AgenciaDto>>(agencias));
        }
        [HttpPost("guardarAgenciaConArchivos")]
        public async Task<IActionResult> GuardarAgenciaConArchivos([FromForm] AgenciaDto dto, [FromForm] List<IFormFile> archivos)
        {
            try
            {
                if (dto == null)
                    return BadRequest("La información de la agencia es requerida.");

                // Puedes validar si el RUC es nulo o inválido
                if (string.IsNullOrWhiteSpace(dto.Ruc))
                    return BadRequest("El RUC de la agencia es obligatorio.");

                var creada = await _service.CrearAgenciaConArchivosAsync(dto, archivos);
                return Ok(creada);
            }
            catch (Exception ex)
            {
                // Aquí podrías loguear con ILogger si lo tienes configurado
                return StatusCode(500, new { mensaje = "Error al guardar la agencia", detalle = ex.Message });
            }
        }


        [HttpPut("Editar/{id}")]
        public async Task<ActionResult<AgenciaDto>> Actualizar(int id, AgenciaDto dto)
        {
            try
            {
                var actualizado = await _service.ActualizarAgenciaAsync(id, dto);
                return Ok(actualizado);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.EliminarAgenciaAsync(id);
            if (!eliminado)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{agenciaId}/formatos")]
        public async Task<IActionResult> ObtenerFormatosPorAgencia(int agenciaId)
        {
            try
            {
                var formatos = await _service.ObtenerFormatosPorAgenciaAsync(agenciaId);
                return Ok(formatos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los formatos", detalle = ex.Message });
            }
        }

    }
}
