using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;

namespace BackendFormatos.Services
{
    public interface IFormatoGenerator
    {
        Task<(byte[] FileBytes, string FileName, string ContentType)> GenerarAsync(
            GenerarFormatoDto dto, AgenciaFormatos plantilla);
    }
}
