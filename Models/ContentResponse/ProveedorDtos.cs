namespace BackendFormatos.Models.ContentResponse
{
    public class ProveedorDtos
    {

    }

    public sealed class ProveedorListDto
    {
        public int ProveedorId { get; set; }
        public string TipoProveedor { get; set; } = null!;
        public string RUC { get; set; } = null!;
        public string RazonSocial { get; set; } = null!;
        public string? Departamento { get; set; }
        public bool TieneTransporte { get; set; }
        public bool TieneSeguridad { get; set; }
    }

    public sealed class ProveedorBancoDto
    {
        public int ProveedorBancoId { get; set; }
        public string Banco { get; set; } = null!;
        public string? Cuenta { get; set; }
        public string? CCI { get; set; }
        public string? CodigoSwift { get; set; }
        public bool EsPrincipal { get; set; }
    }

    public sealed class ProveedorTransporteDto
    {
        public string? Brevete { get; set; }
        public string? Placa { get; set; }
        public string? MarcaVehiculo { get; set; }
    }

    public sealed class ProveedorSeguridadDto
    {
        public string? SucamecCategoria { get; set; }
        public string? Brevete { get; set; }
        public string? Placa { get; set; }
    }

    public sealed class ProveedorDetailDto
    {
        public int ProveedorId { get; set; }
        public byte TipoProveedorId { get; set; }
        public string TipoProveedor { get; set; } = null!;
        public string RUC { get; set; } = null!;
        public string RazonSocial { get; set; } = null!;
        public string? Direccion { get; set; }
        public string? Distrito { get; set; }
        public string? Provincia { get; set; }
        public string? Departamento { get; set; }
        public string? PartidaSunarp { get; set; }
        public string? RepresentanteLegal { get; set; }
        public string? DniRepresentante { get; set; }
        public string? CargoRepresentante { get; set; }
        public DateTime FechaCreacion { get; set; }

        public ProveedorTransporteDto? Transporte { get; set; }
        public ProveedorSeguridadDto? Seguridad { get; set; }
        public List<ProveedorBancoDto> Bancos { get; set; } = new();
    }

    public sealed class ProveedorBancoCreateDto
    {
        public string Banco { get; set; } = null!;
        public string? Cuenta { get; set; }
        public string? CCI { get; set; }
        public string? CodigoSwift { get; set; }
        public bool EsPrincipal { get; set; } = false;
    }

    public sealed class ProveedorCreateDto
    {
        public byte TipoProveedorId { get; set; }         // 1..4
        public string RUC { get; set; } = null!;
        public string RazonSocial { get; set; } = null!;
        public string? Direccion { get; set; }
        public string? Distrito { get; set; }
        public string? Provincia { get; set; }
        public string? Departamento { get; set; }
        public string? PartidaSunarp { get; set; }
        public string? RepresentanteLegal { get; set; }
        public string? DniRepresentante { get; set; }
        public string? CargoRepresentante { get; set; }

        // Datos específicos opcionales
        public ProveedorTransporteDto? Transporte { get; set; }
        public ProveedorSeguridadDto? Seguridad { get; set; }

        public List<ProveedorBancoCreateDto> Bancos { get; set; } = new();
    }

    public sealed class ProveedorCreateConArchivosDto
    {
        // 🔹 mismos campos que ProveedorCreateDto
        public byte TipoProveedorId { get; set; }         // 1..4
        public string RUC { get; set; } = null!;
        public string RazonSocial { get; set; } = null!;
        public string? Direccion { get; set; }
        public string? Distrito { get; set; }
        public string? Provincia { get; set; }
        public string? Departamento { get; set; }
        public string? PartidaSunarp { get; set; }
        public string? RepresentanteLegal { get; set; }
        public string? DniRepresentante { get; set; }
        public string? CargoRepresentante { get; set; }

        // 🔹 estos llegan como JSON en el form-data
        public string? TransporteJson { get; set; }
        public string? SeguridadJson { get; set; }
        public string? BancosJson { get; set; }

        // 🔹 archivos
        public List<IFormFile> Archivos { get; set; } = new();
    }

    public sealed class ProveedorUpdateDto
    {
        public string RazonSocial { get; set; } = null!;
        public string? Direccion { get; set; }
        public string? Distrito { get; set; }
        public string? Provincia { get; set; }
        public string? Departamento { get; set; }
        public string? PartidaSunarp { get; set; }
        public string? RepresentanteLegal { get; set; }
        public string? DniRepresentante { get; set; }
        public string? CargoRepresentante { get; set; }

        public ProveedorTransporteDto? Transporte { get; set; }  // reemplazo/merge según tu lógica
        public ProveedorSeguridadDto? Seguridad { get; set; }

        public List<ProveedorBancoCreateDto>? Bancos { get; set; }  // reemplazo/merge según tu lógica
    }
}
