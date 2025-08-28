using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;

namespace BackendFormatos.Services
{
    public interface IDocumentoParte
    {
        Task<DocumentoParteDto> GetByIdAsync(int id);
        Task<IEnumerable<DocumentoParte>> ObtenerDocumentoParteAsync();
    }
}
