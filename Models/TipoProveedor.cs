using System.ComponentModel.DataAnnotations;

namespace BackendFormatos.Models
{
    public class TipoProveedor
    {
        [Key]
        public byte TipoProveedorId { get; set; }       // 1=ADMINISTRADOR, 2=TRANSPORTISTA, 3=SEGURIDAD, 4=INGENIERIA
        public string Nombre { get; set; } = null!;

        // Navigation
        public ICollection<Proveedor> Proveedores { get; set; } = new List<Proveedor>();
    }
}
