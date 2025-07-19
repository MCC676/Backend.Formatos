using BackendFormatos.Models.ContentResponse;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using Xceed.Words.NET;

namespace BackendFormatos.Services
{
    public class FormatoService : IFormatoService
    {
        private readonly IClienteService _clienteService;
        private readonly IExportadorService _exportadorService;
        private readonly IAgenciaService _agenciaService;

        public FormatoService(IClienteService clienteService,
                          IExportadorService exportadorService,
                          IAgenciaService agenciaService)
        {
            _clienteService = clienteService;
            _exportadorService = exportadorService;
            _agenciaService = agenciaService;
        }

    public async Task<byte[]> GenerarDocumentoWordAsync(GenerarFormatoDto dto)
    {
        var cliente = await _clienteService.GetByIdAsync(dto.ClienteId);
        //var exportador = await _exportadorService.GetByIdAsync(dto.ExportadorId);
        //var agencia = await _agenciaService.GetByIdAsync(dto.AgenciaId);

        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Formato1.docx");
        var outputPath = Path.Combine(Path.GetTempPath(), $"formato-{Guid.NewGuid()}.docx");

        // Cargar documento con DocX
        var doc = DocX.Load(templatePath);

        // Reemplazos
        var placeholders = new Dictionary<string, string>
        {
            { "{{cliente}}", cliente.Nombre },
            { "{{ruc}}", cliente.Ruc },
            { "{{nroFactura}}", dto.NroFactura },
            //{ "{{nombreMina}}", exportador.NombreExportadores },
            { "{{direccion}}", cliente.DomicilioFiscal },
            { "{{representante}}", cliente.RepresentanteLegal },
            { "{{dni}}", cliente.DNIRepresentanteLegal },
            { "{{departamento}}", cliente.Departamento ?? "" },
            { "{{provincia}}", cliente.Provincia ?? "" },
            { "{{distrito}}", cliente.Distrito ?? "" },
        };

        foreach (var pair in placeholders)
        {
            doc.ReplaceText(pair.Key, pair.Value ?? "");
        }

        // Guardar como nuevo documento
        doc.SaveAs(outputPath);

        return await File.ReadAllBytesAsync(outputPath);
    }

    }
}
