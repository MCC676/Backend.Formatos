using BackendFormatos.Data;
using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using Xceed.Words.NET;
using static System.Net.Mime.MediaTypeNames;

namespace BackendFormatos.Services
{
    public class FormatoService : IFormatoService
    {
        private readonly IClienteService _clienteService;
        private readonly IExportadorService _exportadorService;
        private readonly IAgenciaService _agenciaService;
        private readonly IDocumentoParte _documentoParte;
        private readonly DbFormatoContext _context;

        public FormatoService(IClienteService clienteService,
                          IExportadorService exportadorService,
                          IAgenciaService agenciaService,
                          DbFormatoContext context,
                          IDocumentoParte documentoParte)
        {
            _clienteService = clienteService;
            _exportadorService = exportadorService;
            _agenciaService = agenciaService;
            _context = context;
            _documentoParte = documentoParte;
        }

        public async Task<(byte[] FileBytes, string FileName)> GenerarDocumentoExcelAsync(GenerarFormatoDto dto)
        {
            // 1) Ubicar plantilla
            var plantilla = await _context.AgenciaFormatos
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(f => f.Id == dto.FormatoId)
                                 ?? throw new ArgumentException($"No existe formato con Id={dto.FormatoId}");

            if (!File.Exists(plantilla.RutaArchivo!))
                throw new FileNotFoundException("Archivo de plantilla no encontrado", plantilla.RutaArchivo);

            // 2) Datos del cliente y formateo de fecha
            var cliente = await _clienteService.GetByIdAsync(dto.ClienteId);
            // 4) Datos del documento parte
            var docParte = await _documentoParte.GetByIdAsync(dto.DocumentoParteId);

            // 5) formateo de fecha
            var culturaEsPe = new CultureInfo("es-PE");

            if (!DateTime.TryParseExact(dto.Fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out var fechaActa))
            {
                fechaActa = DateTime.Today;
            }

            var fechaLarga = fechaActa.ToString("d 'de' MMMM 'del' yyyy", culturaEsPe); // 18 de julio del 2025
            var ciudad = string.IsNullOrWhiteSpace(dto.Departamento) ? "Lima" : dto.Departamento.Trim();
            var lugarFecha = $"{ciudad}, {fechaLarga}";

            var esPE = CultureInfo.GetCultureInfo("es-PE");

            // 3) Mapa de placeholders (strings)
            var mapTexto = new Dictionary<string, string?>
            {
                ["{{cliente}}"] = cliente.Nombre,
                ["{{ruc}}"] = cliente.Ruc,
                ["{{NroFactura}}"] = dto.NroFactura,
                ["{{direccion}}"] = cliente.DomicilioFiscal,
                ["{{representante}}"] = cliente.RepresentanteLegal,
                ["{{dni}}"] = cliente.DNIRepresentanteLegal,
                ["{{departamento}}"] = cliente.Departamento ?? "",
                ["{{provincia}}"] = cliente.Provincia ?? "",
                ["{{distrito}}"] = cliente.Distrito ?? "",
                ["{{nroRecpo}}"] = cliente.NroRecpo ?? "",
                ["{{fecha}}"] = fechaLarga,
                ["{{fechaLugar}}"] = lugarFecha,
                ["{{CodigoUnico}}"] = cliente.CodigoUnico,
                ["{{NroPartida}}"] = cliente.NroPartida,
                ["{{NroAsiento}}"] = cliente.NroAsiento,
                ["{{AWB}}"] = dto.AWB,
                ["{{SedeProductiva}}"] = cliente.SedeProductiva,
                ["{{NegocioDescripcion}}"] = cliente.NegocioDescripcion,
                ["{{BeneficiariosFinales}}"] = cliente.BeneficiariosFinales,
                ["{{CelularContacto}}"] = cliente.CelularContacto,
                ["{{Correo}}"] = cliente.Correo,
                ["{{NroCuentaCorriente}}"] = cliente.NroCuentaCorriente,
                ["{{Banco}}"] = cliente.Banco,
                ["{{CodigoSwit}}"] = cliente.CodigoSwit,
                //Importador
                ["{{RazonSocial}}"] = docParte.RazonSocial,
                //Datos documento parte
                ["{{NombresApellidos}}"] = docParte.NombresApellidos,
                ["{{Direccion}}"] = docParte.Direccion,
                ["{{Celular}}"] = docParte.Celular,
                ["{{Email}}"] = docParte.Email,
                ["{{Contacto}}"] = docParte.Contacto,
                ["{{ZipCode}}"] = docParte.ZipCode,
                //Datos de la carga a exportar - F
                ["{{DescripcionProducto}}"] = dto.DescripcionProducto,
                ["{{NumeroBultos}}"] = dto.NumeroBultos?.ToString(esPE) ?? "",
                ["{{PesoTotal}}"] = dto.PesoTotal?.ToString("0.###", esPE) ?? "",
                ["{{UnidadPeso}}"] = dto.UnidadPeso,
                ["{{MedioTransporte}}"] = dto.MedioTransporte,
                ["{{ModalidadPagoFlete}}"] = dto.ModalidadPagoFlete,
                ["{{Incoterm}}"] = dto.Incoterm,
                ["{{AWB}}"] = dto.AWB,
                ["{{AeropuertoEmbarque}}"] = dto.AeropuertoEmbarqueNombre,
                ["{{AeropuertoEmbarqueCod}}"] = dto.AeropuertoEmbarqueCod,
                ["{{PaisDestino}}"] = dto.PaisDestinoNombre,
                ["{{PaisDestinoIso2}}"] = dto.PaisDestinoIso2,
                ["{{AeropuertoDestino}}"] = dto.AeropuertoDestinoNombre,
                ["{{AeropuertoDestinoCod}}"] = dto.AeropuertoDestinoCod,
                ["{{CodigoPostalDestino}}"] = dto.CodigoPostalDestino,
                ["{{NumeroPrecinto}}"] = dto.NumeroPrecinto,
                ["{{FechaEmbarque}}"] = dto.FechaEmbarque?.ToString("dd/MM/yyyy", esPE) ?? "",
                //Datos del transportista - F
                ["{{TransportistaNombresApellidos}}"] = dto.TransportistaNombresApellidos,
                ["{{TransportistaCel}}"] = dto.TransportistaCel,
                ["{{TransportistaDni}}"] = dto.TransportistaDni,
                ["{{TransportistaMarca}}"] = dto.TransportistaMarca,
                ["{{TransportistaPlaca}}"] = dto.TransportistaPlaca,
                ["{{TransportistaBrevete}}"] = dto.NumeroBrevete,
                ["{{TransportistaTarjetaPropiedad}}"] = dto.NumeroTarjetaPropiedad,
                ["{{HoraIngresoDepositoTemporal}}"] = dto.HoraIngresoDepositoTemporal?.ToString(@"hh\:mm") ?? "",
                // Datos del mineral
                ["{{pesoBrutoOroDore}}"] = dto.pesoBrutoOroDore,
                ["{{purezaPlata}}"] = dto.purezaPlata,
                ["{{purezaOroDore}}"] = dto.purezaOroDore,
                ["{{rateOnzOro}}"] = dto.rateOnzOro,
                ["{{rateOnzPlata}}"] = dto.rateOnzPlata,
                ["{{rateOnzCobre}}"] = dto.rateOnzCobre
            };

            // (Opcional) Mapa "tipado" para que ciertas celdas queden como número/fecha si la celda contiene SOLO ese placeholder
            // Ejemplo: si en la plantilla pones exactamente {{montoTotal}} en una celda, acá lo grabamos como decimal:
            var mapTipado = new Dictionary<string, object?>
            {
                // ["{{montoTotal}}"] = dto.MontoTotal, // decimal?
                // ["{{fechaActa}}"]  = fechaActa,      // DateTime -> dará formato de fecha si aplicas formato en la celda
            };

            // 4) Abrir y reemplazar con ClosedXML
            using var wb = new XLWorkbook(plantilla.RutaArchivo);

            foreach (var ws in wb.Worksheets)
            {
                // 4.1 Reemplazo embebido en texto (en celdas de texto)
                foreach (var cell in ws.CellsUsed())
                {
                    if (cell.DataType == XLDataType.Text)
                    {
                        var text = cell.GetString();
                        if (string.IsNullOrEmpty(text)) continue;

                        // Reemplazo embebido
                        foreach (var kv in mapTexto)
                        {
                            if (text.Contains(kv.Key, StringComparison.Ordinal))
                            {
                                text = text.Replace(kv.Key, kv.Value ?? string.Empty, StringComparison.Ordinal);
                            }
                        }
                        cell.Value = text;

                        // Reemplazo "tipado" si la celda es EXACTAMENTE el placeholder
                        foreach (var kv in mapTipado)
                        {
                            if (text.Equals(kv.Key, StringComparison.Ordinal))
                            {
                                // Tipos comunes:
                                if (kv.Value is DateTime dt)
                                {
                                    cell.Value = dt;
                                    // Mantén o aplica formato deseado en la plantilla (ej: "dd \"de\" mmmm \"del\" yyyy")
                                }
                                else if (kv.Value is IFormattable || kv.Value is int || kv.Value is long || kv.Value is decimal || kv.Value is double)
                                {
                                    cell.Value = (XLCellValue)kv.Value;
                                    // El formato numérico ya lo puedes definir en la celda en la plantilla
                                }
                                else
                                {
                                    cell.Value = kv.Value?.ToString() ?? string.Empty;
                                }
                            }
                        }
                    }
                }

                // 4.2 (Opcional) Reemplazar en encabezado/pie de página
                // ClosedXML permite texto simple en encabezado/pie:
                // var hdr = ws.PageSetup.Header.Left.AddText(ws.PageSetup.Header.Left.Text.Replace("{{cliente}}", cliente.Nombre));
                // Repite según Left/Center/Right de Header y Footer si lo usas.
                // Nota: Si no usas header/footer con placeholders, puedes omitir.
                void ReplaceHeaderFooter(ref string? s)
                {
                    if (string.IsNullOrEmpty(s)) return;
                    foreach (var kv in mapTexto)
                        s = s.Replace(kv.Key, kv.Value ?? string.Empty, StringComparison.Ordinal);
                }
                static string ReplaceAll(string s, IDictionary<string, string?> map)
                {
                    if (string.IsNullOrEmpty(s)) return s;
                    foreach (var kv in map)
                        s = s.Replace(kv.Key, kv.Value ?? string.Empty, StringComparison.Ordinal);
                    return s;
                }

                // --- Header ---
                var header = ws.PageSetup.Header;

                // Left
                var hl = header.Left.ToString();               // lee el contenido
                hl = ReplaceAll(hl, mapTexto);                 // reemplaza placeholders
                header.Left.Clear();                           // limpia el item
                header.Left.AddText(hl);                       // escribe el nuevo texto

                // Center
                var hc = header.Center.ToString();
                hc = ReplaceAll(hc, mapTexto);
                header.Center.Clear();
                header.Center.AddText(hc);

                // Right
                var hr = header.Right.ToString();
                hr = ReplaceAll(hr, mapTexto);
                header.Right.Clear();
                header.Right.AddText(hr);

                // --- Footer ---
                var footer = ws.PageSetup.Footer;

                // Left
                var fl = footer.Left.ToString();
                fl = ReplaceAll(fl, mapTexto);
                footer.Left.Clear();
                footer.Left.AddText(fl);

                // Center
                var fc = footer.Center.ToString();
                fc = ReplaceAll(fc, mapTexto);
                footer.Center.Clear();
                footer.Center.AddText(fc);

                // Right
                var fr = footer.Right.ToString();
                fr = ReplaceAll(fr, mapTexto);
                footer.Right.Clear();
                footer.Right.AddText(fr);
            }

            // 5) Guardar el resultado en carpeta permanente
            var fileName = $"{Path.GetFileNameWithoutExtension(plantilla.NombreArchivo)}-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var finalDir = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedDocs", dto.ClienteId.ToString(), dto.AgenciaId.ToString());
            Directory.CreateDirectory(finalDir);
            var finalPath = Path.Combine(finalDir, fileName);

            wb.SaveAs(finalPath);

            // 6) Registrar en BD
            _context.FormatosGenerados.Add(new FormatoGenerado
            {
                ClienteId = dto.ClienteId,
                AgenciaId = dto.AgenciaId,
                FormatoId = dto.FormatoId,
                RutaArchivo = finalPath,
                NroFactura = dto.NroFactura,
                FechaGeneracion = DateTime.Now
            });
            await _context.SaveChangesAsync();

            // 7) Devolver bytes
            var bytes = await File.ReadAllBytesAsync(finalPath);
            return (bytes, fileName);
        }

        public async Task<(byte[] FileBytes, string FileName)> GenerarDocumentoWordAsync(GenerarFormatoDto dto)
        {
            var plantilla = await _context.AgenciaFormatos
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(f => f.Id == dto.FormatoId)
                                 ?? throw new ArgumentException($"No existe formato con Id={dto.FormatoId}");

            if (!File.Exists(plantilla.RutaArchivo!))
                throw new FileNotFoundException("Archivo de plantilla no encontrado", plantilla.RutaArchivo);

            var cliente = await _clienteService.GetByIdAsync(dto.ClienteId);

            var culturaEsPe = new CultureInfo("es-PE");

            DateTime fechaActa;
            if (!DateTime.TryParseExact(dto.Fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out fechaActa))
            {
                // fallback: hoy o lanza excepción según tu política
                fechaActa = DateTime.Today;
                // throw new ArgumentException("Formato de fecha inválido. Esperado yyyy-MM-dd.");
            }

            // 2) Construir los dos formatos
            string fechaLarga = fechaActa.ToString("d 'de' MMMM 'del' yyyy", culturaEsPe);           // 18 de julio del 2025
            string ciudad = string.IsNullOrWhiteSpace(dto.Departamento) ? "Lima" : dto.Departamento.Trim();
            string lugarFecha = $"{ciudad}, {fechaLarga}";
            //var fechaHoy = $"Lima, {DateTime.Today:dd 'de' MMMM 'del' yyyy}".ToLower(culturaEsPe);

            using var doc = DocX.Load(plantilla.RutaArchivo);

            var map = new Dictionary<string, string?>
            {
                ["{{cliente}}"] = cliente.Nombre,
                ["{{ruc}}"] = cliente.Ruc,
                ["{{nroFactura}}"] = dto.NroFactura,
                ["{{direccion}}"] = cliente.DomicilioFiscal,
                ["{{representante}}"] = cliente.RepresentanteLegal,
                ["{{dni}}"] = cliente.DNIRepresentanteLegal,
                ["{{departamento}}"] = cliente.Departamento ?? "",
                ["{{provincia}}"] = cliente.Provincia ?? "",
                ["{{distrito}}"] = cliente.Distrito ?? "",
                ["{{nroRecpo}}"] = cliente.NroRecpo ?? "",
                ["{{fecha}}"] = fechaLarga,
                ["{{fechaLugar}}"] = lugarFecha,
                ["{{CodigoUnico}}"] = cliente.CodigoUnico,
                ["{{NroPartida}}"] = cliente.NroPartida,
                ["{{NroAsiento}}"] = cliente.NroAsiento,
                ["{{AWB}}"] = dto.AWB,
                ["{{SedeProductiva}}"] = cliente.SedeProductiva,
            };
            foreach (var (ph, val) in map) doc.ReplaceText(ph, val ?? "");

            // 1️⃣ Guarda el archivo en un directorio permanente
            var fileName = $"{Path.GetFileNameWithoutExtension(plantilla.NombreArchivo)}-{DateTime.Now:yyyyMMddHHmmss}.docx";
            var finalDir = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedDocs", dto.ClienteId.ToString(), dto.AgenciaId.ToString());
            Directory.CreateDirectory(finalDir);
            var finalPath = Path.Combine(finalDir, fileName);
            doc.SaveAs(finalPath);

            // 2️⃣ Inserta registro en FormatoGenerado
            _context.FormatosGenerados.Add(new FormatoGenerado
            {
                ClienteId = dto.ClienteId,
                AgenciaId = dto.AgenciaId,
                FormatoId = dto.FormatoId,
                RutaArchivo = finalPath,
                NroFactura = dto.NroFactura,
                FechaGeneracion = DateTime.Now
            });
            await _context.SaveChangesAsync();

            // 3️⃣ Devuelve bytes para descarga inmediata
            var bytes = await File.ReadAllBytesAsync(finalPath);
            return (bytes, fileName);
        }

        public async Task<IEnumerable<FormatoGeneradoDto>> GetHistorialAsync(int? clienteId = null, int? agenciaId = null)
        {
            var q = from fg in _context.FormatosGenerados
                    join c in _context.Clientes on fg.ClienteId equals c.Id
                    join a in _context.Agencias on fg.AgenciaId equals a.Id
                    join f in _context.AgenciaFormatos on fg.FormatoId equals f.Id
                    select new FormatoGeneradoDto
                    {
                        Id = fg.Id,
                        Cliente = c.Nombre,
                        Agencia = a.NombreAgencias,
                        Formato = f.NombreArchivo,
                        Fecha = fg.FechaGeneracion,
                        Ruta = fg.RutaArchivo
                    };

            //if (clienteId.HasValue) q = q.Where(r => r.ClienteId == clienteId);   // agrega ClienteId al DTO si vas a filtrar
            //if (agenciaId.HasValue) q = q.Where(r => r.AgenciaId == agenciaId);

            return await q.OrderByDescending(r => r.Fecha).ToListAsync();
        }


    }
}
