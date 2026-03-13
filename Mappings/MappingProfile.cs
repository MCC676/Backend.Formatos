using AutoMapper;
using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;

namespace BackendFormatos.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Clientes, ClienteDto>();
            CreateMap<ClienteDto, Clientes>();

            CreateMap<Agencias, AgenciaDto>();
            CreateMap<AgenciaDto, Agencias>();

            CreateMap<Exportadores, ExportadorDto>();
            CreateMap<ExportadorDto, Exportadores>();

            CreateMap<DocumentoParte, DocumentoParteDto>();
            CreateMap<DocumentoParteDto, DocumentoParte>();

            // ==========================================
            // NUEVOS: Proveedores (normalizado)
            // ==========================================

            // Listado básico
            CreateMap<Proveedor, ProveedorListDto>()
                .ForMember(d => d.TipoProveedor, m => m.MapFrom(s => s.TipoProveedor.Nombre))
                .ForMember(d => d.TieneTransporte, m => m.MapFrom(s => s.Transporte != null))
                .ForMember(d => d.TieneSeguridad, m => m.MapFrom(s => s.Seguridad != null));

            // Detalle completo
            CreateMap<Proveedor, ProveedorDetailDto>()
                .ForMember(d => d.TipoProveedor, m => m.MapFrom(s => s.TipoProveedor.Nombre));

            CreateMap<ProveedorTransporte, ProveedorTransporteDto>();
            CreateMap<ProveedorSeguridad, ProveedorSeguridadDto>();
            CreateMap<ProveedorBanco, ProveedorBancoDto>();

            // Create
            CreateMap<ProveedorCreateDto, Proveedor>();
            CreateMap<ProveedorTransporteDto, ProveedorTransporte>();
            CreateMap<ProveedorSeguridadDto, ProveedorSeguridad>();
            CreateMap<ProveedorBancoCreateDto, ProveedorBanco>();

            // Update (ajusta según política de merge/reemplazo de colecciones)
            CreateMap<ProveedorUpdateDto, Proveedor>();
        }
    }
}
