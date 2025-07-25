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

}
