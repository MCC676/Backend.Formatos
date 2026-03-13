namespace BackendFormatos.Models
{
    public class ProveedorBanco
    {
        public int ProveedorBancoId { get; set; }
        public int ProveedorId { get; set; }

        public string Banco { get; set; } = null!;
        public string? Cuenta { get; set; }
        public string? CCI { get; set; }
        public string? CodigoSwift { get; set; }
        public bool EsPrincipal { get; set; }

        // Navigation
        public Proveedor Proveedor { get; set; } = null!;
    }
}
