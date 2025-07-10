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
    }
}
