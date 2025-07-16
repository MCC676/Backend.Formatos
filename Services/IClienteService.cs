using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;

namespace BackendFormatos.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<Clientes>> ObtenerClientesAsync();
        Task<ClienteDto> GetByIdAsync(int id);
        Task<ClienteDto> CrearCliente(ClienteDto cliente);
    }
}
