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


        [HttpPut("actualizarAgencia/{id}")]
        public async Task<IActionResult> ActualizarAgenciaConArchivos(int id, [FromBody] AgenciaDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("La información es requerida.");

                var actualizada = await _service.ActualizarAgenciaConArchivosAsync(id, dto);
                return Ok(actualizada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la agencia", detalle = ex.Message });
            }
        }


        [HttpDelete("eliminarAgencia/{id}")]
        public async Task<IActionResult> EliminarAgencia(int id)
        {
            try
            {
                await _service.EliminarAgenciaAsync(id);
                return Ok(new { mensaje = "Agencia eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar agencia", detalle = ex.Message });
            }
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

        [HttpDelete("eliminarFormato/{id}")]
        public async Task<IActionResult> EliminarFormato(int id)
        {
            try
            {
                await _service.EliminarFormatoAsync(id);
                return Ok(new { mensaje = "Formato eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar formato", detalle = ex.Message });
            }
        }

        [HttpPost("agregarArchivos/{id}")]
        public async Task<IActionResult> AgregarArchivos(int id, [FromForm] List<IFormFile> archivos)
        {
            try
            {
                await _service.AgregarArchivosAgenciaAsync(id, archivos);
                return Ok(new { mensaje = "Archivos agregados correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al subir archivos", detalle = ex.Message });
            }
        }

    }
}
