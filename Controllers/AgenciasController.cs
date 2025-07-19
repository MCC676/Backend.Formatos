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
        [HttpPost("guardarAgencia")]
        public async Task<ActionResult<AgenciaDto>> CrearAgencia(AgenciaDto dto)
        {
            try
            {
                var creada = await _service.CrearAgenciaAsync(dto);
                return Ok(creada); // o usar CreatedAtAction si ya tienes GetById
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la agencia", detalle = ex.Message });
            }
        }
    }
}
