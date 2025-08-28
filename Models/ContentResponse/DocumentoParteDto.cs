namespace BackendFormatos.Models.ContentResponse
{
    public class DocumentoParteDto
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public string? Rol { get; set; }
        public string? RazonSocial { get; set; }
        public string? NombresApellidos { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? EstadoProvincia { get; set; }
        public string? Pais { get; set; }
        public string? ZipCode { get; set; }
        public string? Celular { get; set; }
        public string? Email { get; set; }
        public string? Contacto { get; set; }
        public string? Activo { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
