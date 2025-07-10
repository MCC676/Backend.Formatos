using BackendFormatos.Models.ContentResponse;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

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
    var exportador = await _exportadorService.GetByIdAsync(dto.ExportadorId);
    var agencia = await _agenciaService.GetByIdAsync(dto.AgenciaId);

    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Formato1.docx");
    var outputPath = Path.Combine(Path.GetTempPath(), $"formato-{Guid.NewGuid()}.docx");

    File.Copy(templatePath, outputPath, true);

    // Diccionario de campos a reemplazar
    var placeholders = new Dictionary<string, string>
    {
        { "{{cliente}}", cliente.Nombre },
        { "{{ruc}}", cliente.Ruc },
        { "{{nroFactura}}", dto.NroFactura },
        { "{{nombreMina}}", exportador.NombreExportadores }
    };

    using (var wordDoc = WordprocessingDocument.Open(outputPath, true))
    {
        var documentText = string.Join("", wordDoc.MainDocumentPart.Document.Body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>().Select(t => t.Text));

        foreach (var pair in placeholders)
        {
            documentText = documentText.Replace(pair.Key, pair.Value ?? "");
        }

        // Borra el contenido original del body
        wordDoc.MainDocumentPart.Document.Body.RemoveAllChildren();

        // Inserta todo como un único párrafo
        var newParagraph = new Paragraph(new Run(new DocumentFormat.OpenXml.Wordprocessing.Text(documentText)));
        wordDoc.MainDocumentPart.Document.Body.AppendChild(newParagraph);

        wordDoc.MainDocumentPart.Document.Save();
    }

    return await File.ReadAllBytesAsync(outputPath);
}

    }
}
