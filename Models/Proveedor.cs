namespace BackendFormatos.Models
{
    public class Proveedor
    {
        public int ProveedorId { get; set; }
        public byte TipoProveedorId { get; set; }

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

        // Navigation
        public TipoProveedor TipoProveedor { get; set; } = null!;
        public ProveedorTransporte? Transporte { get; set; }        // 1:1 opcional
        public ProveedorSeguridad? Seguridad { get; set; }          // 1:1 opcional
        public ICollection<ProveedorBanco> Bancos { get; set; } = new List<ProveedorBanco>();
    }
}
