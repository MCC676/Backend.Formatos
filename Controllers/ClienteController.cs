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
    public class ClienteController : _BaseController
    {
        private readonly IClienteService _service;
        private readonly IMapper _mapper;
        public ClienteController(DbFormatoContext context, IClienteService service, IMapper mapper)
        : base(context)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> Listar()
        {
            var clientes = await _service.ObtenerClientesAsync();
            return Ok(_mapper.Map<List<ClienteDto>>(clientes));
        }
    }
}
