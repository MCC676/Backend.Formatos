using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;
using Microsoft.AspNetCore.Mvc;

namespace BackendFormatos.Services
{
    public interface IProveedorService
    {
        // Tipos
        Task<List<KeyValuePair<byte, string>>> ListarTiposAsync();

        // Proveedores (listar/filtrar)
        Task<List<ProveedorListDto>> ListarAsync(byte? tipoProveedorId = null); // si null, trae todos
        Task<List<ProveedorDetailDto>> ListarTodosAsync();
        Task<ProveedorDetailDto?> ObtenerDetalleAsync(int proveedorId);

        // CRUD Proveedor
        Task<ProveedorDetailDto> CrearAsync(ProveedorCreateDto dto);
        Task<ProveedorDetailDto> CrearProveedorConArchivosAsync(ProveedorCreateDto dto, List<IFormFile> archivos);
        Task<ProveedorDetailDto> ActualizarAsync(int proveedorId, ProveedorUpdateDto dto);
        Task<bool> EliminarAsync(int proveedorId);

        // Bancos
        Task<List<ProveedorBancoDto>> ListarBancosAsync(int proveedorId);
        Task<ProveedorBancoDto> AgregarBancoAsync(int proveedorId, ProveedorBancoCreateDto dto);
        Task<bool> EliminarBancoAsync(int proveedorBancoId);
        Task<bool> MarcarBancoPrincipalAsync(int proveedorId, int proveedorBancoId);

        // Transporte / Seguridad (1:1)
        Task<ProveedorTransporteDto?> UpsertTransporteAsync(int proveedorId, ProveedorTransporteDto dto);
        Task<ProveedorSeguridadDto?> UpsertSeguridadAsync(int proveedorId, ProveedorSeguridadDto dto);


        Task<ProveedorDetailDto> ActualizarAsync(int id, ProveedorCreateDto dto);
        Task<ProveedorDetailDto> ActualizarConArchivosAsync(int id, ProveedorCreateDto dto, List<IFormFile> archivos);

        Task<List<ProveedorFormatos>> ObtenerFormatosPorProveedorAsync(int agenciaId);

        Task EliminarFormatoAsync(int id);
        Task EliminarProveedorAsync(int id);
        Task AgregarArchivosProveedorAsync(int proveedorId, List<IFormFile> archivos);

    }
}
