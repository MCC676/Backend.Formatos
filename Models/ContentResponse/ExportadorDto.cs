namespace BackendFormatos.Models.ContentResponse
{
    public class ExportadorDto
    {
        public int Id { get; set; }
        public string? NombreExportadores { get; set; }
        public string? Direccion { get; set; }
        public string? Ruc { get; set; }
        public bool estado { get; set; }
    }
}
