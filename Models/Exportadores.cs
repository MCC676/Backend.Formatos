using BackendFormatos.Models.ContentResponse;
using System.ComponentModel.DataAnnotations;

namespace BackendFormatos.Models
{
    public class Exportadores
    {
        public int Id { get; set; }

        [Required]
        public string? NombreExportadores { get; set; }

        [Required]
        public string? Direccion { get; set; }

        [Required]
        public string? Ruc { get; set; }

        public bool Estado { get; set; }
        public string Tipo { get; set; } = "ShipTo";
        public string? Ciudad { get; set; }
        public string? Pais { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Contacto { get; set; }

    }
}
