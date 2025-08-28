using BackendFormatos.Data;
using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;
using DocumentFormat.OpenXml.Drawing.Charts;
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

            if (archivos is { Count: > 0 })
            {
                // ✅ Ruta base (sug: pasar a appsettings y leerlo con IOptions)
                var rutaBase = @"C:\Formato";
                var rutaAgencia = Path.Combine(rutaBase, dto.Ruc);
                Directory.CreateDirectory(rutaAgencia);

                // ✅ Listas blancas
                var extPermitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".doc", ".docx", ".pdf",
            ".xls", ".xlsx", ".xlsm", ".xltx", ".csv"
        };

                var mimePermitidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/pdf",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-excel.sheet.macroEnabled.12",
            "text/csv"
        };

                // Límite (MB). Si Excel puede ser pesado, súbelo aquí.
                const int maxSizeMb = 25;
                const long maxSizeBytes = maxSizeMb * 1024L * 1024L;

                foreach (var archivo in archivos)
                {
                    if (archivo == null || archivo.Length == 0) continue;

                    var nombreOriginal = Path.GetFileName(archivo.FileName);
                    var ext = Path.GetExtension(nombreOriginal);

                    // ✅ Validaciones básicas
                    if (string.IsNullOrEmpty(ext) || !extPermitidas.Contains(ext))
                        throw new InvalidOperationException($"Tipo de archivo no permitido: {nombreOriginal}");

                    if (archivo.Length > maxSizeBytes)
                        throw new InvalidOperationException($"{nombreOriginal} excede {maxSizeMb} MB.");

                    // Algunos navegadores no envían bien el MIME; no lo hacemos bloqueante
                    if (!string.IsNullOrEmpty(archivo.ContentType) && !mimePermitidos.Contains(archivo.ContentType))
                    {
                        // No rechazamos si viene vacío o raro, pero podrías hacerlo estricto si quieres:
                        // throw new InvalidOperationException($"MIME no permitido: {archivo.ContentType}");
                    }

                    // ✅ Sanitizar nombre y generar nombre único para evitar colisiones
                    var baseName = Path.GetFileNameWithoutExtension(nombreOriginal);
                    baseName = string.Join("_", baseName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
                    var nombreUnico = $"{baseName}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";

                    var rutaDestino = Path.Combine(rutaAgencia, nombreUnico);

                    // ✅ Guardar a disco
                    using (var stream = new FileStream(rutaDestino, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await archivo.CopyToAsync(stream);
                    }

                    // ✅ Registrar en BD
                    var archivoDb = new AgenciaFormatos
                    {
                        AgenciaId = agencia.Id,
                        NombreArchivo = nombreUnico, // guardamos el nombre único
                        RutaArchivo = rutaDestino,
                        FechaRegistro = DateTime.Now,
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

        public async Task<AgenciaDto> ActualizarAgenciaConArchivosAsync(int id, AgenciaDto dto)
        {
            var agencia = await _context.Agencias.FindAsync(id);
            if (agencia == null)
                throw new Exception("Agencia no encontrada");

            agencia.NombreAgencias = dto.NombreAgencias;
            agencia.Direccion = dto.Direccion;
            agencia.Ruc = dto.Ruc;
            agencia.Estado = dto.Estado;

            _context.Agencias.Update(agencia);
            await _context.SaveChangesAsync();

            dto.Id = agencia.Id;
            return dto;
        }

        public async Task EliminarAgenciaAsync(int id)
        {
            var agencia = await _context.Agencias.FindAsync(id);
            if (agencia == null)
                throw new Exception("Agencia no encontrada");

            var formatos = await _context.AgenciaFormatos
                .Where(f => f.AgenciaId == id)
                .ToListAsync();

            // Eliminar archivos físicos del disco
            foreach (var archivo in formatos)
            {
                if (System.IO.File.Exists(archivo.RutaArchivo))
                    System.IO.File.Delete(archivo.RutaArchivo);

                _context.AgenciaFormatos.Remove(archivo);
            }

            // Eliminar la agencia
            _context.Agencias.Remove(agencia);
            await _context.SaveChangesAsync();

            // (Opcional) Eliminar la carpeta si queda vacía
            var rutaCarpeta = Path.Combine(@"C:\Formato", agencia.Ruc);
            if (Directory.Exists(rutaCarpeta) && !Directory.EnumerateFileSystemEntries(rutaCarpeta).Any())
            {
                Directory.Delete(rutaCarpeta);
            }
        }

        public async Task EliminarFormatoAsync(int id)
        {
            var formato = await _context.AgenciaFormatos.FindAsync(id);

            if (formato == null)
                throw new Exception("Formato no encontrado");

            if (System.IO.File.Exists(formato.RutaArchivo))
                System.IO.File.Delete(formato.RutaArchivo);

            _context.AgenciaFormatos.Remove(formato);
            await _context.SaveChangesAsync();

            // (Opcional) limpiar carpeta si está vacía
            var rutaCarpeta = Path.GetDirectoryName(formato.RutaArchivo);
            if (Directory.Exists(rutaCarpeta) && !Directory.EnumerateFileSystemEntries(rutaCarpeta).Any())
            {
                Directory.Delete(rutaCarpeta);
            }
        }

        public async Task AgregarArchivosAgenciaAsync(int agenciaId, List<IFormFile> archivos)
        {
            var agencia = await _context.Agencias.FindAsync(agenciaId);
            if (agencia == null)
                throw new Exception("Agencia no encontrada");

            if (archivos != null && archivos.Any())
            {
                var rutaBase = @"C:\Formato";
                var rutaAgencia = Path.Combine(rutaBase, agencia.Ruc);
                Directory.CreateDirectory(rutaAgencia);

                foreach (var archivo in archivos)
                {
                    var nombre = Path.GetFileName(archivo.FileName);
                    var rutaCompleta = Path.Combine(rutaAgencia, nombre);

                    using var stream = new FileStream(rutaCompleta, FileMode.Create);
                    await archivo.CopyToAsync(stream);

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
        }
    }
}
