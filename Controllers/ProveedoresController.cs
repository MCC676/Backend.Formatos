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
    public class ProveedoresController : _BaseController
    {
        private readonly IProveedorService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ProveedoresController> _logger;

        public ProveedoresController(DbFormatoContext context, IProveedorService service, IMapper mapper, ILogger<ProveedoresController> logger)
            : base(context)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        // Tipos (para combos de filtro)
        [HttpGet("tipos")]
        public async Task<ActionResult<IEnumerable<object>>> GetTipos()
        {
            var tipos = await _service.ListarTiposAsync();
            return Ok(tipos.Select(t => new { id = t.Key, nombre = t.Value }));
        }

        // Listar (opcionalmente por tipo: 1=Admin, 2=Transp, 3=Seg, 4=Ing)
        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<ProveedorListDto>>> Listar([FromQuery] byte? tipoId = null)
        {
            var data = await _service.ListarAsync(tipoId);
            return Ok(data);
        }

        [HttpGet("listarCompleto")]
        public async Task<ActionResult<List<ProveedorListDto>>> ListarTodos()
        {
            try
            {
                var proveedores = await _service.ListarTodosAsync();
                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listando proveedores");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }


        // Detalle
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProveedorDetailDto>> Detalle(int id)
        {
            var dto = await _service.ObtenerDetalleAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost("crear")]
        public async Task<ActionResult<ProveedorDetailDto>> Crear([FromBody] ProveedorCreateDto dto)
        {
            try
            {
                var created = await _service.CrearAsync(dto);
                return CreatedAtAction(nameof(Detalle), new { id = created.ProveedorId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error creando proveedor");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("crear-con-archivos")]
        public async Task<ActionResult<ProveedorDetailDto>> CrearConArchivos( [FromForm] ProveedorCreateDto dto,  [FromForm] List<IFormFile> archivos)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "La información del proveedor es requerida." });

                if (string.IsNullOrWhiteSpace(dto.RUC))
                    return BadRequest(new { message = "El RUC del proveedor es obligatorio." });

                var creado = await _service.CrearProveedorConArchivosAsync(dto, archivos);

                // Igual que tu Crear normal: devolvemos CreatedAtAction
                return CreatedAtAction(nameof(Detalle), new { id = creado.ProveedorId }, creado);
            }
            catch (InvalidOperationException ex)
            {
                // ej: RUC duplicado
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando proveedor con archivos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("actualizarProveedor/{id}")]
        public async Task<ActionResult<ProveedorDetailDto>> Actualizar(
                    int id,
                    [FromBody] ProveedorCreateDto dto)
        {
            try
            {
                var actualizado = await _service.ActualizarAsync(id, dto);
                return Ok(actualizado);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Proveedor no encontrado" });
            }
            catch (InvalidOperationException ex)
            {
                // Ej: RUC duplicado
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando proveedor");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("actualizar-con-archivos/{id}")]
        public async Task<ActionResult<ProveedorDetailDto>> ActualizarConArchivos(int id,[FromForm] ProveedorCreateDto dto,[FromForm] List<IFormFile> archivos)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "La información del proveedor es requerida." });

                if (string.IsNullOrWhiteSpace(dto.RUC))
                    return BadRequest(new { message = "El RUC del proveedor es obligatorio." });

                var actualizado = await _service.ActualizarConArchivosAsync(id, dto, archivos);
                return Ok(actualizado);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Proveedor no encontrado" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando proveedor con archivos");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{proveedorId}/formatos")]
        public async Task<IActionResult> ObtenerFormatosPorProveedor(int proveedorId)
        {
            try
            {
                var formatos = await _service.ObtenerFormatosPorProveedorAsync(proveedorId);
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

        [HttpDelete("eliminarProveedor/{id}")]
        public async Task<IActionResult> EliminarProveedor(int id)
        {
            try
            {
                await _service.EliminarProveedorAsync(id);
                return Ok(new { mensaje = "Proveedor eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar Proveedor", detalle = ex.Message });
            }
        }

        // Actualizar
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProveedorDetailDto>> Actualizar(int id, [FromBody] ProveedorUpdateDto dto)
        {
            var updated = await _service.ActualizarAsync(id, dto);
            return Ok(updated);
        }

        // Eliminar
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var ok = await _service.EliminarAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        // ===== Bancos =====

        [HttpGet("{proveedorId:int}/bancos")]
        public async Task<ActionResult<IEnumerable<ProveedorBancoDto>>> ListarBancos(int proveedorId)
        {
            var data = await _service.ListarBancosAsync(proveedorId);
            return Ok(data);
        }

        [HttpPost("{proveedorId:int}/bancos")]
        public async Task<ActionResult<ProveedorBancoDto>> AgregarBanco(int proveedorId, [FromBody] ProveedorBancoCreateDto dto)
        {
            var created = await _service.AgregarBancoAsync(proveedorId, dto);
            return Ok(created);
        }

        [HttpDelete("bancos/{proveedorBancoId:int}")]
        public async Task<ActionResult> EliminarBanco(int proveedorBancoId)
        {
            var ok = await _service.EliminarBancoAsync(proveedorBancoId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPut("{proveedorId:int}/bancos/{proveedorBancoId:int}/principal")]
        public async Task<ActionResult> MarcarBancoPrincipal(int proveedorId, int proveedorBancoId)
        {
            var ok = await _service.MarcarBancoPrincipalAsync(proveedorId, proveedorBancoId);
            if (!ok) return NotFound();
            return NoContent();
        }

        // ===== Transporte / Seguridad =====

        [HttpPut("{proveedorId:int}/transporte")]
        public async Task<ActionResult<ProveedorTransporteDto>> UpsertTransporte(int proveedorId, [FromBody] ProveedorTransporteDto dto)
        {
            var r = await _service.UpsertTransporteAsync(proveedorId, dto);
            return Ok(r);
        }

        [HttpPut("{proveedorId:int}/seguridad")]
        public async Task<ActionResult<ProveedorSeguridadDto>> UpsertSeguridad(int proveedorId, [FromBody] ProveedorSeguridadDto dto)
        {
            var r = await _service.UpsertSeguridadAsync(proveedorId, dto);
            return Ok(r);
        }


        [HttpPost("agregarArchivos/{id}")]
        public async Task<IActionResult> AgregarArchivos(int id, [FromForm] List<IFormFile> archivos)
        {
            try
            {
                await _service.AgregarArchivosProveedorAsync(id, archivos);
                return Ok(new { mensaje = "Archivos agregados correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al subir archivos", detalle = ex.Message });
            }
        }
    }
}
