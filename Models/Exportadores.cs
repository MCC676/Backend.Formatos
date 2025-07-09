using System.ComponentModel.DataAnnotations;

namespace BackendFormatos.Models
{
    public class Exportadores
    {
        public int Id { get; set; }

        [Required]
        public string? NombreExportador { get; set; }

        [Required]
        public string? Direccion { get; set; }

        [Required]
        public string? Ruc { get; set; }

        public bool Estado { get; set; }

      
    }
}
