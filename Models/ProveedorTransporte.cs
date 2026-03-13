using System.ComponentModel.DataAnnotations;

namespace BackendFormatos.Models
{
    public class ProveedorTransporte
    {
        // PK = FK a Proveedor (1:1)
        [Key]
        public int ProveedorId { get; set; }

        public string? Brevete { get; set; }
        public string? Placa { get; set; }
        public string? MarcaVehiculo { get; set; }

        // Navigation
        public Proveedor Proveedor { get; set; } = null!;
    }
}
