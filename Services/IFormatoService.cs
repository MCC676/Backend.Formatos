using BackendFormatos.Models.ContentResponse;

namespace BackendFormatos.Services
{
    public interface IFormatoService
    {
        Task<byte[]> GenerarDocumentoWordAsync(GenerarFormatoDto dto);
    }
}
