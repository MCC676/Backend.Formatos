using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackendFormatos.Data;
using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;
using Microsoft.EntityFrameworkCore;

namespace BackendFormatos.Services
{
    public class ProveedorService : _BaseService, IProveedorService
    {
        private readonly IMapper _mapper;

        public ProveedorService(DbFormatoContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<List<KeyValuePair<byte, string>>> ListarTiposAsync()
        {
            return await _context.TipoProveedor
                .AsNoTracking()
                .OrderBy(t => t.TipoProveedorId)
                .Select(t => new KeyValuePair<byte, string>(t.TipoProveedorId, t.Nombre))
                .ToListAsync();
        }

        public async Task<List<ProveedorListDto>> ListarAsync(byte? tipoProveedorId = null)
        {
            var q = _context.Proveedor
                .AsNoTracking()
                .Include(p => p.TipoProveedor)
                .Include(p => p.Transporte)
                .Include(p => p.Seguridad)
                .OrderBy(p => p.TipoProveedorId).ThenBy(p => p.RazonSocial)
                .AsQueryable();

            if (tipoProveedorId.HasValue)
                q = q.Where(p => p.TipoProveedorId == tipoProveedorId.Value);

            return await q.ProjectTo<ProveedorListDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<List<ProveedorDetailDto>> ListarTodosAsync()
        {
            // Suponiendo que tus entidades son Proveedor y ProveedorBanco
            // y que la navegación es Proveedor.ProveedorBancos
            return await _context.Proveedor
                .Include(p => p.Bancos) // nombre de la navegación en tu entity
                .Select(p => new ProveedorDetailDto
                {
                    ProveedorId = p.ProveedorId,
                    TipoProveedorId = p.TipoProveedorId,
                    RUC = p.RUC,
                    RazonSocial = p.RazonSocial,
                    Direccion = p.Direccion,
                    Distrito = p.Distrito,
                    Provincia = p.Provincia,
                    Departamento = p.Departamento,
                    RepresentanteLegal = p.RepresentanteLegal,
                    DniRepresentante = p.DniRepresentante,
                    CargoRepresentante = p.CargoRepresentante,

                    Bancos = p.Bancos
                        .Select(b => new ProveedorBancoDto
                        {
                            ProveedorBancoId = b.ProveedorBancoId, // si existe
                            Banco = b.Banco,
                            Cuenta = b.Cuenta,
                            CCI = b.CCI,
                            CodigoSwift = b.CodigoSwift,
                            EsPrincipal = b.EsPrincipal
                        })
                        .ToList()
                })
                .OrderBy(p => p.RazonSocial)   // opcional
                .ToListAsync();
        }

        public async Task<ProveedorDetailDto?> ObtenerDetalleAsync(int proveedorId)
        {
            return await _context.Proveedor
                .AsNoTracking()
                .Include(p => p.TipoProveedor)
                .Include(p => p.Transporte)
                .Include(p => p.Seguridad)
                .Include(p => p.Bancos)
                .Where(p => p.ProveedorId == proveedorId)
                .ProjectTo<ProveedorDetailDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<ProveedorDetailDto> CrearAsync(ProveedorCreateDto dto)
        {
            // Validación más específica
            if (await _context.Proveedor.AnyAsync(x => x.RUC == dto.RUC))
                throw new InvalidOperationException($"Ya existe un proveedor con RUC {dto.RUC}");

            // Usar transacción para asegurar consistencia
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = _mapper.Map<Proveedor>(dto);
                _context.Proveedor.Add(entity);
                await _context.SaveChangesAsync();

                await CrearTransporteSiAplica(entity.ProveedorId, dto.Transporte);
                await CrearSeguridadSiAplica(entity.ProveedorId, dto.Seguridad);
                await CrearBancos(entity.ProveedorId, dto.Bancos);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await ObtenerDetalleOrThrow(entity.ProveedorId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ProveedorDetailDto> CrearProveedorConArchivosAsync( ProveedorCreateDto dto, List<IFormFile> archivos)
        {
            // 🔹 Primero crea todo el proveedor con su lógica actual (transporte, seguridad, bancos, etc.)
            var proveedorCreado = await CrearAsync(dto); // usa tu método ya existente

            // Si no hay archivos, simplemente devolvemos el dto creado
            if (archivos is not { Count: > 0 })
                return proveedorCreado;

            // 🔹 Ruta base para proveedores
            var rutaBase = @"C:\Formato\Proveedor"; // luego lo pasas a appsettings si quieres
            var rutaProveedor = Path.Combine(rutaBase, dto.RUC);
            Directory.CreateDirectory(rutaProveedor);

            // ✅ Listas blancas de extensión y MIME (igual que en Agencia)
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

            const int maxSizeMb = 25;
            const long maxSizeBytes = maxSizeMb * 1024L * 1024L;

            foreach (var archivo in archivos)
            {
                if (archivo == null || archivo.Length == 0)
                    continue;

                var nombreOriginal = Path.GetFileName(archivo.FileName);
                var ext = Path.GetExtension(nombreOriginal);

                // 🔍 Validación de extensión
                if (string.IsNullOrEmpty(ext) || !extPermitidas.Contains(ext))
                    throw new InvalidOperationException($"Tipo de archivo no permitido: {nombreOriginal}");

                // 🔍 Validación de tamaño
                if (archivo.Length > maxSizeBytes)
                    throw new InvalidOperationException($"{nombreOriginal} excede {maxSizeMb} MB.");

                // MIME no lo hacemos bloqueante, igual que en Agencia
                if (!string.IsNullOrEmpty(archivo.ContentType) && !mimePermitidos.Contains(archivo.ContentType))
                {
                    // Podrías lanzar error si quieres ser más estricto
                }

                // 🔐 Sanitizar nombre + nombre único
                var baseName = Path.GetFileNameWithoutExtension(nombreOriginal);
                baseName = string.Join("_", baseName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

                var nombreUnico = $"{baseName}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
                var rutaDestino = Path.Combine(rutaProveedor, nombreUnico);

                // 💾 Guardar en disco
                using (var stream = new FileStream(rutaDestino, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await archivo.CopyToAsync(stream);
                }

                // 🗄️ Registrar en BD (ajusta al nombre de tu entidad de documentos)
                var doc = new ProveedorFormatos
                {
                    ProveedorId = proveedorCreado.ProveedorId,  // viene de tu ProveedorDetailDto
                    NombreArchivo = nombreUnico,
                    RutaArchivo = rutaDestino,
                    FechaRegistro = DateTime.Now
                };

                _context.ProveedorFormatos.Add(doc);
            }

            await _context.SaveChangesAsync();

            return proveedorCreado;
        }


        private async Task CrearTransporteSiAplica(int proveedorId, ProveedorTransporteDto transporteDto)
        {
            if (transporteDto == null) return;

            var transporte = _mapper.Map<ProveedorTransporte>(transporteDto);
            transporte.ProveedorId = proveedorId;
            _context.ProveedorTransporte.Add(transporte);
        }

        private async Task CrearSeguridadSiAplica(int proveedorId, ProveedorSeguridadDto seguridadDto)
        {
            if (seguridadDto == null) return;

            var seguridad = _mapper.Map<ProveedorSeguridad>(seguridadDto);
            seguridad.ProveedorId = proveedorId;
            _context.ProveedorSeguridad.Add(seguridad);
        }

        private async Task CrearBancos(int proveedorId, List<ProveedorBancoCreateDto> bancosDto)
        {
            if (bancosDto?.Count == 0) return;

            var bancos = bancosDto.Select((b, index) =>
            {
                var banco = _mapper.Map<ProveedorBanco>(b);
                banco.ProveedorId = proveedorId;

                // Lógica más clara para EsPrincipal
                if (!bancosDto.Any(x => x.EsPrincipal))
                    banco.EsPrincipal = index == 0;

                return banco;
            }).ToList();

            _context.ProveedorBanco.AddRange(bancos);
        }

        public async Task<ProveedorDetailDto> ActualizarAsync(int id, ProveedorCreateDto dto)
        {
            // Validar RUC duplicado (excluyendo el propio)
            if (await _context.Proveedor.AnyAsync(x => x.RUC == dto.RUC && x.ProveedorId != id))
                throw new InvalidOperationException($"Ya existe un proveedor con RUC {dto.RUC}");

            // Cargar proveedor + relaciones
            var proveedor = await _context.Proveedor
                .Include(p => p.Bancos)
                // .Include(p => p.Transporte)
                // .Include(p => p.Seguridad)
                .FirstOrDefaultAsync(p => p.ProveedorId == id);

            if (proveedor == null)
                throw new KeyNotFoundException("Proveedor no encontrado");

            // 🔹 Actualizar campos principales
            proveedor.TipoProveedorId = dto.TipoProveedorId;  // o 1 fijo si así lo decides
            proveedor.RUC = dto.RUC;
            proveedor.RazonSocial = dto.RazonSocial;
            proveedor.Direccion = dto.Direccion;
            proveedor.Distrito = dto.Distrito;
            proveedor.Provincia = dto.Provincia;
            proveedor.Departamento = dto.Departamento;
            proveedor.PartidaSunarp = dto.PartidaSunarp;
            proveedor.RepresentanteLegal = dto.RepresentanteLegal;
            proveedor.DniRepresentante = dto.DniRepresentante;
            proveedor.CargoRepresentante = dto.CargoRepresentante;
            // proveedor.Estado           = dto.Estado; // si tienes este campo en entity

            // 🔹 Actualizar transporte / seguridad (según tu diseño)
           // await ActualizarTransporteSiAplica(proveedor.ProveedorId, dto.Transporte);
           // await ActualizarSeguridadSiAplica(proveedor.ProveedorId, dto.Seguridad);

            // 🔹 Actualizar bancos: versión simple = borro todos y recreo
            _context.ProveedorBanco.RemoveRange(proveedor.Bancos);

            if (dto.Bancos != null && dto.Bancos.Count > 0)
            {
                foreach (var bancoDto in dto.Bancos)
                {
                    var banco = new ProveedorBanco
                    {
                        ProveedorId = proveedor.ProveedorId,
                        Banco = bancoDto.Banco,
                        Cuenta = bancoDto.Cuenta,
                        CCI = bancoDto.CCI,
                        CodigoSwift = bancoDto.CodigoSwift,
                        EsPrincipal = bancoDto.EsPrincipal
                    };
                    _context.ProveedorBanco.Add(banco);
                }
            }

            await _context.SaveChangesAsync();

            // 🔹 Devolver el detalle actualizado
            return await ObtenerDetalleOrThrow(proveedor.ProveedorId);
        }

        public async Task<ProveedorDetailDto> ActualizarConArchivosAsync(int id,ProveedorCreateDto dto,List<IFormFile> archivos)
        {
            // Primero actualizamos datos “normales”
            var proveedorActualizado = await ActualizarAsync(id, dto);

            // Si no hay archivos nuevos, devolvemos tal cual
            if (archivos is not { Count: > 0 })
                return proveedorActualizado;

            // Ruta base
            var rutaBase = @"C:\Formato\proveedor";
            var rutaProveedor = Path.Combine(rutaBase, dto.RUC);
            Directory.CreateDirectory(rutaProveedor);

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

            const int maxSizeMb = 25;
            const long maxSizeBytes = maxSizeMb * 1024L * 1024L;

            foreach (var archivo in archivos)
            {
                if (archivo == null || archivo.Length == 0)
                    continue;

                var nombreOriginal = Path.GetFileName(archivo.FileName);
                var ext = Path.GetExtension(nombreOriginal);

                if (string.IsNullOrEmpty(ext) || !extPermitidas.Contains(ext))
                    throw new InvalidOperationException($"Tipo de archivo no permitido: {nombreOriginal}");

                if (archivo.Length > maxSizeBytes)
                    throw new InvalidOperationException($"{nombreOriginal} excede {maxSizeMb} MB.");

                if (!string.IsNullOrEmpty(archivo.ContentType) && !mimePermitidos.Contains(archivo.ContentType))
                {
                    // opcional: lanzar error si quieres ser más estricto
                }

                var baseName = Path.GetFileNameWithoutExtension(nombreOriginal);
                baseName = string.Join("_", baseName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
                var nombreUnico = $"{baseName}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";

                var rutaDestino = Path.Combine(rutaProveedor, nombreUnico);

                using (var stream = new FileStream(rutaDestino, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await archivo.CopyToAsync(stream);
                }

                // Registrar en tabla de documentos de proveedor
                var doc = new ProveedorFormatos
                {
                    ProveedorId = proveedorActualizado.ProveedorId,
                    NombreArchivo = nombreUnico,
                    RutaArchivo = rutaDestino,
                    FechaRegistro = DateTime.Now
                };

                _context.ProveedorFormatos.Add(doc);
            }

            await _context.SaveChangesAsync();

            // Devolver de nuevo el detalle, ya con documentos nuevos
            return await ObtenerDetalleOrThrow(id);
        }


        public async Task<ProveedorDetailDto> ActualizarAsync(int proveedorId, ProveedorUpdateDto dto)
        {
            var entity = await _context.Proveedor
                .Include(p => p.Transporte)
                .Include(p => p.Seguridad)
                .Include(p => p.Bancos)
                .SingleOrDefaultAsync(p => p.ProveedorId == proveedorId);

            if (entity == null) throw new KeyNotFoundException("Proveedor no encontrado");

            // Actualiza campos simples
            _mapper.Map(dto, entity);

            // Upsert transporte / seguridad según ingreso
            if (dto.Transporte != null)
            {
                if (entity.Transporte == null)
                {
                    var t = _mapper.Map<ProveedorTransporte>(dto.Transporte);
                    t.ProveedorId = proveedorId;
                    _context.ProveedorTransporte.Add(t);
                }
                else
                {
                    _mapper.Map(dto.Transporte, entity.Transporte);
                }
            }

            if (dto.Seguridad != null)
            {
                if (entity.Seguridad == null)
                {
                    var s = _mapper.Map<ProveedorSeguridad>(dto.Seguridad);
                    s.ProveedorId = proveedorId;
                    _context.ProveedorSeguridad.Add(s);
                }
                else
                {
                    _mapper.Map(dto.Seguridad, entity.Seguridad);
                }
            }

            // Bancos (política simple: si se envían, reemplaza todos)
            if (dto.Bancos != null)
            {
                var current = await _context.ProveedorBanco.Where(b => b.ProveedorId == proveedorId).ToListAsync();
                _context.ProveedorBanco.RemoveRange(current);

                var algunoPrincipal = dto.Bancos.Any(b => b.EsPrincipal);
                foreach (var b in dto.Bancos)
                {
                    var be = _mapper.Map<ProveedorBanco>(b);
                    be.ProveedorId = proveedorId;
                    if (!algunoPrincipal)
                        be.EsPrincipal = (b == dto.Bancos.First()); // marca el primero por defecto
                    _context.ProveedorBanco.Add(be);
                }
            }

            await _context.SaveChangesAsync();

            return await ObtenerDetalleOrThrow(proveedorId);
        }


        public async Task<List<ProveedorFormatos>> ObtenerFormatosPorProveedorAsync(int agenciaId)
        {
            var formatos = await _context.ProveedorFormatos
                .Where(f => f.ProveedorId == agenciaId)
                .Select(f => new ProveedorFormatos
                {
                    Id = f.Id,
                    NombreArchivo = f.NombreArchivo,
                    RutaArchivo = f.RutaArchivo,
                    FechaRegistro = f.FechaRegistro
                })
                .ToListAsync();

            return formatos;
        }

        public async Task EliminarFormatoAsync(int id)
        {
            var formato = await _context.ProveedorFormatos.FindAsync(id);

            if (formato == null)
                throw new Exception("Formato no encontrado");

            if (System.IO.File.Exists(formato.RutaArchivo))
                System.IO.File.Delete(formato.RutaArchivo);

            _context.ProveedorFormatos.Remove(formato);
            await _context.SaveChangesAsync();

            // (Opcional) limpiar carpeta si está vacía
            var rutaCarpeta = Path.GetDirectoryName(formato.RutaArchivo);
            if (Directory.Exists(rutaCarpeta) && !Directory.EnumerateFileSystemEntries(rutaCarpeta).Any())
            {
                Directory.Delete(rutaCarpeta);
            }
        }

        public async Task EliminarProveedorAsync(int id)
        {
            var prov = await _context.Proveedor.FindAsync(id);
            if (prov == null)
                throw new Exception("Proveedor no encontrada");

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
            _context.Proveedor.Remove(prov);
            await _context.SaveChangesAsync();

            // (Opcional) Eliminar la carpeta si queda vacía
            var rutaCarpeta = Path.Combine(@"C:\Formato\Proveedor", prov.RazonSocial);
            if (Directory.Exists(rutaCarpeta) && !Directory.EnumerateFileSystemEntries(rutaCarpeta).Any())
            {
                Directory.Delete(rutaCarpeta);
            }
        }


        public async Task AgregarArchivosProveedorAsync(int proveedorId, List<IFormFile> archivos)
        {
            var agencia = await _context.Agencias.FindAsync(proveedorId);
            if (agencia == null)
                throw new Exception("Agencia no encontrada");

            if (archivos != null && archivos.Any())
            {
                var rutaBase = @"C:\Formato\Proveedor";
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

        public async Task<bool> EliminarAsync(int proveedorId)
        {
            var entity = await _context.Proveedor.FindAsync(proveedorId);
            if (entity == null) return false;

            _context.Proveedor.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProveedorBancoDto>> ListarBancosAsync(int proveedorId)
        {
            return await _context.ProveedorBanco
                .AsNoTracking()
                .Where(b => b.ProveedorId == proveedorId)
                .OrderByDescending(b => b.EsPrincipal)
                .ThenBy(b => b.Banco)
                .ProjectTo<ProveedorBancoDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ProveedorBancoDto> AgregarBancoAsync(int proveedorId, ProveedorBancoCreateDto dto)
        {
            var existsProv = await _context.Proveedor.AnyAsync(p => p.ProveedorId == proveedorId);
            if (!existsProv) throw new KeyNotFoundException("Proveedor no encontrado");

            if (dto.EsPrincipal)
            {
                // Apagar principal existente
                await _context.ProveedorBanco
                    .Where(b => b.ProveedorId == proveedorId && b.EsPrincipal)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.EsPrincipal, false));
            }

            var entity = _mapper.Map<ProveedorBanco>(dto);
            entity.ProveedorId = proveedorId;

            // Si ninguno es principal, marca este si es el primero
            var hayBancos = await _context.ProveedorBanco.AnyAsync(b => b.ProveedorId == proveedorId);
            if (!hayBancos && dto.EsPrincipal == false) entity.EsPrincipal = true;

            _context.ProveedorBanco.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProveedorBancoDto>(entity);
        }

        public async Task<bool> EliminarBancoAsync(int proveedorBancoId)
        {
            var entity = await _context.ProveedorBanco.FindAsync(proveedorBancoId);
            if (entity == null) return false;

            _context.ProveedorBanco.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarcarBancoPrincipalAsync(int proveedorId, int proveedorBancoId)
        {
            var banco = await _context.ProveedorBanco
                .SingleOrDefaultAsync(b => b.ProveedorBancoId == proveedorBancoId && b.ProveedorId == proveedorId);

            if (banco == null) return false;

            // Apaga otros
            await _context.ProveedorBanco
                .Where(b => b.ProveedorId == proveedorId && b.ProveedorBancoId != proveedorBancoId && b.EsPrincipal)
                .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.EsPrincipal, false));

            // Marca este
            banco.EsPrincipal = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProveedorTransporteDto?> UpsertTransporteAsync(int proveedorId, ProveedorTransporteDto dto)
        {
            var prov = await _context.Proveedor
                .Include(p => p.Transporte)
                .SingleOrDefaultAsync(p => p.ProveedorId == proveedorId);

            if (prov == null) throw new KeyNotFoundException("Proveedor no encontrado");

            if (prov.Transporte == null)
            {
                var t = _mapper.Map<ProveedorTransporte>(dto);
                t.ProveedorId = proveedorId;
                _context.ProveedorTransporte.Add(t);
            }
            else
            {
                _mapper.Map(dto, prov.Transporte);
            }

            await _context.SaveChangesAsync();

            var tDto = await _context.ProveedorTransporte
                .Where(t => t.ProveedorId == proveedorId)
                .ProjectTo<ProveedorTransporteDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            return tDto;
        }

        public async Task<ProveedorSeguridadDto?> UpsertSeguridadAsync(int proveedorId, ProveedorSeguridadDto dto)
        {
            var prov = await _context.Proveedor
                .Include(p => p.Seguridad)
                .SingleOrDefaultAsync(p => p.ProveedorId == proveedorId);

            if (prov == null) throw new KeyNotFoundException("Proveedor no encontrado");

            if (prov.Seguridad == null)
            {
                var s = _mapper.Map<ProveedorSeguridad>(dto);
                s.ProveedorId = proveedorId;
                _context.ProveedorSeguridad.Add(s);
            }
            else
            {
                _mapper.Map(dto, prov.Seguridad);
            }

            await _context.SaveChangesAsync();

            var sDto = await _context.ProveedorSeguridad
                .Where(s => s.ProveedorId == proveedorId)
                .ProjectTo<ProveedorSeguridadDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            return sDto;
        }

        private async Task<ProveedorDetailDto> ObtenerDetalleOrThrow(int proveedorId)
        {
            var dto = await ObtenerDetalleAsync(proveedorId);
            if (dto == null) throw new InvalidOperationException("No se pudo cargar el detalle del proveedor");
            return dto;
        }
    }
}
