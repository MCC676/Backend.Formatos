using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;

namespace BackendFormatos.Services
{
    public interface IAgenciaService
    {
        Task<IEnumerable<Agencias>> ObtenerAgenciasAsync();
        Task<AgenciaDto> GetByIdAsync(int id);
        Task<AgenciaDto> CrearAgenciaConArchivosAsync(AgenciaDto dto, List<IFormFile> formFile);
        Task<AgenciaDto> ActualizarAgenciaConArchivosAsync(int id, AgenciaDto dto);
        Task EliminarAgenciaAsync(int id);
        Task<List<AgenciaFormatoDto>> ObtenerFormatosPorAgenciaAsync(int agenciaId);
        Task AgregarArchivosAgenciaAsync(int agenciaId, List<IFormFile> archivos);
        Task EliminarFormatoAsync(int id);
    }
}
