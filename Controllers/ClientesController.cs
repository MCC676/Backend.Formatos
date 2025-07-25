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

        [HttpPost("guardarCliente")]
        public async Task<ActionResult<ClienteDto>> CrearCliente(ClienteDto clienteDto)
        {
            try
            {
                var clienteCreado = await _service.CrearCliente(clienteDto);
                return Ok(clienteCreado);
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

        [HttpPut("editarCliente/{id}")]
        public async Task<IActionResult> Actualizar(int id, ClienteDto dto)
        {
            try
            {
                var cliente = await _service.ActualizarClienteAsync(id, dto);
                return Ok(cliente);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.EliminarClienteAsync(id);
            if (!eliminado)
                return NotFound();

            return NoContent();
        }
    }
}
