namespace BackendFormatos.Models.ContentResponse
{
    public class GenerarFormatoDto
    {
        public int ClienteId { get; set; }
        //public int ExportadorId { get; set; }
        //public int AgenciaId { get; set; }

        public string? Fecha { get; set; }
        public string? FechaExpiracion { get; set; }
        public string? NroFactura { get; set; }
        public string? Placa { get; set; }
        public string? FormatoSeleccionado { get; set; }

        // Datos descriptivos para el Word
        public string? ClienteNombre { get; set; }
        public string? ClienteRuc { get; set; }
        public string? ClienteDireccion { get; set; }

        public string? Representante { get; set; }
        public string? RepresentanteDni { get; set; }

        public string? CodigoMina { get; set; }
        public string? NombreMina { get; set; }
        public string? Departamento { get; set; }
        public string? Provincia { get; set; }
        public string? Distrito { get; set; }
    }

}
