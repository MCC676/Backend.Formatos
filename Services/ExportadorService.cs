using BackendFormatos.Data;
using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;
using Microsoft.EntityFrameworkCore;

namespace BackendFormatos.Services
{
    public class ExportadorService : IExportadorService
    {
        private readonly DbFormatoContext _context;

        public ExportadorService(DbFormatoContext context)
        {
            _context = context;
        }

        public async Task<ExportadorDto> GetByIdAsync(int id)
        {
            var entity = await _context.Exportadores.FindAsync(id);
        if (entity == null) throw new Exception("Exportador no encontrado");

        return new ExportadorDto
        {
            Id = entity.Id,
            NombreExportadores = entity.NombreExportadores,
        };
        }

        public async Task<IEnumerable<Exportadores>> ObtenerExportadoresAsync()
        {
            return await _context.Exportadores.Where(e => e.Estado == true).ToListAsync();
        }
    }
}
