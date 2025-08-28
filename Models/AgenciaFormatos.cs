namespace BackendFormatos.Models
{
    public class AgenciaFormatos
    {
        public int Id { get; set; }
        public int AgenciaId { get; set; }
        public string? NombreArchivo { get; set; }
        public string? RutaArchivo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public Agencias? Agencia { get; set; }
    }

    public class FormatoGenerado
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int AgenciaId { get; set; }
        public int FormatoId { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string RutaArchivo { get; set; } = default!;
        public string? NroFactura { get; set; }

        public Clientes? Cliente { get; set; }
        public Agencias? Agencia { get; set; }
        public AgenciaFormatos? Formato { get; set; }
    }

}
