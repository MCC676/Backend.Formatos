using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;

namespace BackendFormatos.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<Clientes>> ObtenerClientesAsync();
    }
}
