using System.ComponentModel.DataAnnotations;

namespace BackendFormatos.Models
{
    public class Agencias
    {
        public int Id { get; set; }

        [Required]
        public string? NombreAgencias { get; set; }

        [Required]
        public string? Direccion { get; set; }

        [Required]
        public string? Ruc { get; set; }

        public bool Estado { get; set; }
        public ICollection<AgenciaFormatos>? Formatos { get; set; }
    }
}
