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
        }
    }
}
