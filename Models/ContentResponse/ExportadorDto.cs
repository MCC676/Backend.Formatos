namespace BackendFormatos.Models.ContentResponse
{
    public class ExportadorDto
    {
        public int Id { get; set; }
        public string? NombreExportadores { get; set; }
        public string? Direccion { get; set; }
        public string? Ruc { get; set; }
        public bool estado { get; set; }
        public string? Tipo { get; set; }
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Contacto { get; set; }
    }

    public enum ExportadorTipo
    {
        ShipTo,
        BuyerNotify
    }

}
