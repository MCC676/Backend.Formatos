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
        

        public async Task<AgenciaDto> ActualizarAgenciaAsync(int id, AgenciaDto dto)
        {
            try
            {
                var entidad = await _context.Agencias.FindAsync(id);
                if (entidad == null)
                    throw new KeyNotFoundException("Agencia no encontrada");

                entidad.NombreAgencias = dto.NombreAgencias;
                entidad.Direccion = dto.Direccion;
                entidad.Ruc = dto.Ruc;
                entidad.Estado = dto.Estado;

                await _context.SaveChangesAsync();

                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la agencia", ex);
            }
        }

        public async Task<bool> EliminarAgenciaAsync(int id)
        {
            try
            {
                var entidad = await _context.Agencias.FindAsync(id);
                if (entidad == null)
                    return false;

                _context.Agencias.Remove(entidad);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la agencia", ex);
            }
        }

        public async Task<AgenciaDto> CrearAgenciaConArchivosAsync(AgenciaDto dto, List<IFormFile> archivos)
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

            if (archivos != null && archivos.Any())
            {
                // ✅ Ruta base fija en disco C
                var rutaBase = @"C:\Formato";
                var rutaAgencia = Path.Combine(rutaBase, dto.Ruc);

                // ✅ Crear carpeta si no existe
                Directory.CreateDirectory(rutaAgencia);

                foreach (var archivo in archivos)
                {
                    var nombre = Path.GetFileName(archivo.FileName);
                    var rutaCompleta = Path.Combine(rutaAgencia, nombre);

                    // ✅ Guardar archivo
                    using var stream = new FileStream(rutaCompleta, FileMode.Create);
                    await archivo.CopyToAsync(stream);

                    // ✅ Registrar en BD
                    var archivoDb = new AgenciaFormatos
                    {
                        AgenciaId = agencia.Id,
                        NombreArchivo = nombre,
                        RutaArchivo = rutaCompleta,
                        FechaRegistro = DateTime.Now
                    };

                    _context.AgenciaFormatos.Add(archivoDb);
                }

                await _context.SaveChangesAsync();
            }

            dto.Id = agencia.Id;
            return dto;
        }

        public async Task<List<AgenciaFormatoDto>> ObtenerFormatosPorAgenciaAsync(int agenciaId)
        {
            var formatos = await _context.AgenciaFormatos
                .Where(f => f.AgenciaId == agenciaId)
                .Select(f => new AgenciaFormatoDto
                {
                    Id = f.Id,
                    NombreArchivo = f.NombreArchivo,
                    RutaArchivo = f.RutaArchivo,
                    FechaRegistro = f.FechaRegistro
                })
                .ToListAsync();

            return formatos;
        }
    }
}
