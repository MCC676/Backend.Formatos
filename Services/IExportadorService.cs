using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;

namespace BackendFormatos.Services
{
    public interface IExportadorService
    {
        Task<IEnumerable<Exportadores>> ObtenerExportadoresAsync();
        Task<ExportadorDto> GetByIdAsync(int id);
        Task<ExportadorDto> CrearExportadorAsync(ExportadorDto dto);
        Task<ExportadorDto> ActualizarExportadorAsync(int id, ExportadorDto dto);
        Task<bool> EliminarExportadorAsync(int id);
    }
}
