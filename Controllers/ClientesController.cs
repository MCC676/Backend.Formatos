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
    public class ClientesController : _BaseController
    {
        private readonly IClienteService _service;
        private readonly IMapper _mapper;
        public ClientesController(DbFormatoContext context, IClienteService service, IMapper mapper)
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

        [HttpPost]
        public async Task<ActionResult<ClienteDto>> CrearCliente(ClienteDto clienteDto)
        {
            try
            {
                var clienteCreado = await _service.CrearCliente(clienteDto);
                return CreatedAtAction(nameof(_service.GetByIdAsync), new { id = clienteCreado.Id }, clienteCreado);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Loggear el error
                return StatusCode(500, new { message = "Ocurrió un error al procesar la solicitud" });
            }
        }
    }
}
