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
    }
}
