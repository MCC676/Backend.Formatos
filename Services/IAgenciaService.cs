using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;

namespace BackendFormatos.Services
{
    public interface IAgenciaService
    {
        Task<IEnumerable<Agencias>> ObtenerAgenciasAsync();
        Task<AgenciaDto> GetByIdAsync(int id);
    }
}
