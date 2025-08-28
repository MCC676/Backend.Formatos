using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;

namespace BackendFormatos.Services
{
    public interface IFormatoService
    {
        Task<(byte[] FileBytes, string FileName)> GenerarDocumentoExcelAsync(GenerarFormatoDto dto);
        Task<(byte[] FileBytes, string FileName)> GenerarDocumentoWordAsync(GenerarFormatoDto dto);
        Task<IEnumerable<FormatoGeneradoDto>> GetHistorialAsync(int? clienteId = null, int? agenciaId = null);

    }
}
