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
        public async Task<ExportadorDto> CrearExportadorAsync(ExportadorDto dto)
        {
            var entidad = new Exportadores
            {
                NombreExportadores = dto.NombreExportadores,
                Direccion = dto.Direccion,
                Ruc = dto.Ruc
            };

            _context.Exportadores.Add(entidad);
            await _context.SaveChangesAsync();

            dto.Id = entidad.Id;
            return dto;
        }
        public async Task<ExportadorDto> ActualizarExportadorAsync(int id, ExportadorDto dto)
        {
            var entidad = await _context.Exportadores.FindAsync(id);
            if (entidad == null)
                throw new KeyNotFoundException("Exportador no encontrado");

            entidad.NombreExportadores = dto.NombreExportadores;
            entidad.Direccion = dto.Direccion;
            entidad.Ruc = dto.Ruc;
            entidad.Estado = dto.estado;

            await _context.SaveChangesAsync();

            return dto;
        }
        public async Task<bool> EliminarExportadorAsync(int id)
        {
            var entidad = await _context.Exportadores.FindAsync(id);
            if (entidad == null)
                return false;

            _context.Exportadores.Remove(entidad);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
