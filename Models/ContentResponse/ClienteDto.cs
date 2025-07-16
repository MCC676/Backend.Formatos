using System.ComponentModel.DataAnnotations;

namespace BackendFormatos.Models.ContentResponse
{
    public class ClienteDto
    {
        [Required]
        public int Id { get; set; }
        public string? Nombre { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El RUC debe tener 11 caracteres")]
        public string? Ruc { get; set; }

        [Required]
        public string? RazonSocial { get; set; }

        public string? UsuarioSunat { get; set; }
        public string? ClaveSunat { get; set; }
        public DateTime? FechaInscripcion { get; set; }
        public DateTime? InicioActivacion { get; set; }

        [EmailAddress]
        public string? Correo { get; set; }
        public string? ClaveCorreo { get; set; }
        public string? RepresentanteLegal { get; set; }

        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 caracteres")]
        public string? DNIRepresentanteLegal { get; set; }
        public string? Cargo { get; set; }
        public DateTime? CargoDesde { get; set; }
        public string? DomicilioFiscal { get; set; }
        public string? DireccionLima { get; set; }
        public string? CelularContacto { get; set; }
        public string? PRegistral { get; set; }
        public string? Planilla { get; set; }
        public string? VigenciaCExplotacion { get; set; }
        public string? Auto { get; set; }
        public string? Placa { get; set; }
        public string? Brevete { get; set; }
        public string? Conductor { get; set; }
        public int? Cantidad { get; set; }
        public DateTime? VigenciaInscripcion { get; set; }
        public string? NroRecpo { get; set; }
        public string? Condicion { get; set; }
        public string? EstadoReinfo { get; set; }
        public string? NombreConcesion { get; set; }
        public string? Departamento { get; set; }
        public string? Provincia { get; set; }
        public string? Distrito { get; set; }
        public string? CodigoUnico { get; set; }
        public int? CantidadConcesiones { get; set; }
        public decimal? DeusaConcesionesDolares { get; set; }
        public string? Banco { get; set; }
        public string? NroCuentaCorriente { get; set; }
        public string? CCI { get; set; }
        public string? CodigoSwit { get; set; }
        public string? Observaciones { get; set; }
        public bool Estado { get; set; } = true;
    }
}
