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
    }
}
