using AutoMapper;
using BackendFormatos.Data;
using BackendFormatos.Models;
using BackendFormatos.Models.ContentResponse;
using Microsoft.EntityFrameworkCore;

namespace BackendFormatos.Services
{
    public class DocumentoParteService : _BaseService, IDocumentoParte
    {
        private readonly IMapper _mapper;
        public DocumentoParteService(DbFormatoContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }
        public async Task<DocumentoParteDto> GetByIdAsync(int id)
        {
            var entity = await _context.DocumentoParte.FindAsync(id);
            //if (entity == null) throw new Exception("DocumentoParte no encontrado");
            if (entity == null)
            {
                return new DocumentoParteDto(); // todas las propiedades vacías / por defecto
            }


            return new DocumentoParteDto
            {
                Id = entity.Id,
                Tipo = entity.Tipo,
                Rol = entity.Rol,
                RazonSocial = entity.RazonSocial,
                NombresApellidos = entity.NombresApellidos,
                Direccion = entity.Direccion,
                Ciudad = entity.Ciudad,
                EstadoProvincia = entity.EstadoProvincia,
                Pais = entity.Pais,
                ZipCode = entity.ZipCode,
                Celular = entity.Celular,
                Email = entity.Email,
                Contacto = entity.Contacto,
            };
        }

        public async Task<IEnumerable<DocumentoParte>> ObtenerDocumentoParteAsync()
        {
            return await _context.DocumentoParte
            .Where(c => c.Activo == true)
            .OrderBy(c => c.Rol)
            .ToListAsync();
        }
    }
}
