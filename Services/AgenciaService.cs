using BackendFormatos.Data;
using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;
using Microsoft.EntityFrameworkCore;

namespace BackendFormatos.Services
{
    public class AgenciaService : IAgenciaService
    {
        private readonly DbFormatoContext _context;

        public AgenciaService(DbFormatoContext context)
        {
            _context = context;
        }
        public async Task<AgenciaDto> GetByIdAsync(int id)
        {
            var entity = await _context.Agencias.FindAsync(id);
            if (entity == null) throw new Exception("Agencia no encontrada");

            return new AgenciaDto
            {
                Id = entity.Id,
                NombreAgencias = entity.NombreAgencias
                // Agrega más campos si necesitas
            };
        }

        public async Task<IEnumerable<Agencias>> ObtenerAgenciasAsync()
        {
            return await _context.Agencias.Where(a => a.Estado == true).ToListAsync();
        }
        public async Task<AgenciaDto> CrearAgenciaAsync(AgenciaDto dto)
        {
            var agencia = new Agencias
            {
                NombreAgencias = dto.NombreAgencias,
                Direccion = dto.Direccion,
                Ruc = dto.Ruc,
                Estado = dto.Estado
            };

            _context.Agencias.Add(agencia);
            await _context.SaveChangesAsync();

            dto.Id = agencia.Id;
            return dto;
        }
        public Task<AgenciaDto> ActualizarAgenciaAsync(int id, AgenciaDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarAgenciaAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
