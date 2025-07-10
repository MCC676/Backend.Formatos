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
    public class ExportadoresController : _BaseController
    {
        private readonly IExportadorService _service;
        private readonly IMapper _mapper;

        public ExportadoresController(DbFormatoContext context, IExportadorService service, IMapper mapper)
            : base(context)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<ExportadorDto>>> Listar()
        {
            var exportadores = await _service.ObtenerExportadoresAsync();
            return Ok(_mapper.Map<List<ExportadorDto>>(exportadores));
        }
    }
}
