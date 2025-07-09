using System.ComponentModel.DataAnnotations;

namespace BackendFormatos.Models
{
    public class Clientes
    {
        public int Id { get; set; }

        [Required]
        public string? Nombre { get; set; }

        [Required]
        public string? Ruc { get; set; }

        [Required]
        public string? RazonSocial { get; set; }

        [Required]
        public string? UsuarioSunat { get; set; }

        [Required]
        public string? ClaveSunat { get; set; }
        public DateTime? FechaInscripcion { get; set; }
        public DateTime? InicioActivacion { get; set; }
        public string? Correo { get; set; }
        public string? ClaveCorreo { get; set; }
        public string? RepresentanteLegal { get; set; }
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
        public bool Estado { get; set; }
    }
}
