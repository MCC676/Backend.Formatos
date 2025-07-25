using AutoMapper;
using BackendFormatos.Data;
using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;
using Microsoft.EntityFrameworkCore;

namespace BackendFormatos.Services
{
    public class ClienteService : _BaseService, IClienteService
    {
        private readonly IMapper _mapper;
        public ClienteService(DbFormatoContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<ClienteDto> GetByIdAsync(int id)
        {
            var entity = await _context.Clientes.FindAsync(id);
            if (entity == null) throw new Exception("Cliente no encontrado");

            return new ClienteDto
            {
                //Id = entity.Id,
                Nombre = entity.Nombre,
                Ruc = entity.Ruc,
                RepresentanteLegal = entity.RepresentanteLegal,
                DNIRepresentanteLegal = entity.DNIRepresentanteLegal,
                DomicilioFiscal = entity.DomicilioFiscal,
                Departamento = entity.Departamento,
                Provincia = entity.Provincia,
                Distrito = entity.Distrito
            };
        }

        public async Task<IEnumerable<Clientes>> ObtenerClientesAsync()
        {
            return await _context.Clientes
            .Where(c => c.Estado == true)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
        }

        public async Task<ClienteDto> CrearCliente(ClienteDto clienteDto)
        {
            // Validar que el RUC no exista
            var existeRuc = await _context.Clientes.AnyAsync(c => c.Ruc == clienteDto.Ruc);
            if (existeRuc)
            {
                throw new BadHttpRequestException("Ya existe un cliente con este RUC");
            }

            // Mapear DTO a entidad
            var cliente = _mapper.Map<Clientes>(clienteDto);

            // Asignar fecha de creación (si la tienes en tu entidad)
            // cliente.FechaCreacion = DateTime.Now;

            // Guardar en la base de datos
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            // Mapear la entidad guardada a DTO de respuesta
            return _mapper.Map<ClienteDto>(cliente);
        }

        public async Task<ClienteDto> ActualizarClienteAsync(int id, ClienteDto dto)
        {
            var entidad = await _context.Clientes.FindAsync(id);
            if (entidad == null)
                throw new KeyNotFoundException("Cliente no encontrado");

            entidad.Nombre = dto.Nombre;
            entidad.Ruc = dto.Ruc;
            entidad.RazonSocial = dto.RazonSocial;
            entidad.UsuarioSunat = dto.UsuarioSunat;
            entidad.ClaveSunat = dto.ClaveSunat;
            entidad.FechaInscripcion = dto.FechaInscripcion;
            entidad.InicioActivacion = dto.InicioActivacion;
            entidad.Correo = dto.Correo;
            entidad.ClaveCorreo = dto.ClaveCorreo;
            entidad.RepresentanteLegal = dto.RepresentanteLegal;
            entidad.DNIRepresentanteLegal = dto.DNIRepresentanteLegal;
            entidad.Cargo = dto.Cargo;
            entidad.CargoDesde = dto.CargoDesde;
            entidad.DomicilioFiscal = dto.DomicilioFiscal;
            entidad.DireccionLima = dto.DireccionLima;
            entidad.CelularContacto = dto.CelularContacto;
            entidad.PRegistral = dto.PRegistral;
            entidad.Planilla = dto.Planilla;
            entidad.VigenciaCExplotacion = dto.VigenciaCExplotacion;
            entidad.Auto = dto.Auto;
            entidad.Placa = dto.Placa;
            entidad.Brevete = dto.Brevete;
            entidad.Conductor = dto.Conductor;
            entidad.Cantidad = dto.Cantidad;
            entidad.VigenciaInscripcion = dto.VigenciaInscripcion;
            entidad.NroRecpo = dto.NroRecpo;
            entidad.Condicion = dto.Condicion;
            entidad.EstadoReinfo = dto.EstadoReinfo;
            entidad.NombreConcesion = dto.NombreConcesion;
            entidad.Departamento = dto.Departamento;
            entidad.Provincia = dto.Provincia;
            entidad.Distrito = dto.Distrito;
            entidad.CodigoUnico = dto.CodigoUnico;
            entidad.CantidadConcesiones = dto.CantidadConcesiones;
            entidad.DeusaConcesionesDolares = dto.DeusaConcesionesDolares;
            entidad.Banco = dto.Banco;
            entidad.NroCuentaCorriente = dto.NroCuentaCorriente;
            entidad.CCI = dto.CCI;
            entidad.CodigoSwit = dto.CodigoSwit;
            entidad.Observaciones = dto.Observaciones;
            entidad.Estado = dto.Estado;

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> EliminarClienteAsync(int id)
        {
            var entidad = await _context.Clientes.FindAsync(id);
            if (entidad == null)
                return false;

            _context.Clientes.Remove(entidad);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
