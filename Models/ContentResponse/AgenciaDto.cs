using System.ComponentModel.DataAnnotations;

namespace BackendFormatos.Models.ContentResponse
{
    public class AgenciaDto
    {
        public int Id { get; set; }

        [Required]
        public string NombreAgencias { get; set; } = string.Empty;

        [Required]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El RUC debe tener 11 caracteres")]
        public string Ruc { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;
    }

    public class AgenciaFormatoDto
    {
        public int Id { get; set; }
        public string? NombreArchivo { get; set; }
        public string? RutaArchivo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

}
