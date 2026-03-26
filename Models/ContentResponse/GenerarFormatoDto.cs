namespace BackendFormatos.Models.ContentResponse
{
    public class GenerarFormatoDto
    {
        public int FormatoId { get; set; }   // 🔸 NUEVO
        public int ClienteId { get; set; }
        public int ExportadorId { get; set; }
        public int AgenciaId { get; set; }
        public int DocumentoParteId { get; set; }
        public int DocumentoParteConsigneId { get; set; }
        public int DocumentoParteBuyerId { get; set; }
        public string? Fecha { get; set; }
        public string? FechaExpiracion { get; set; }
        public string? NroFactura { get; set; }
        public string? SerieFactura { get; set; }
        public string? Placa { get; set; }

        // placeholders opcionales que ya mandas
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
        public string? NroRecpo { get; set; }
        public string? CodigoUnico { get; set; }
        public string? NroPartida { get; set; }
        public string? NroAsiento { get; set; }
        public string? AWB { get; set; }
        public string? SedeProductiva { get; set; }
        public string? NegocioDescripcion { get; set; }
        public string? BeneficiariosFinales { get; set; }

        //Datos de la carga a exportar
        public string? DescripcionProducto { get; set; }
        public int? NumeroBultos { get; set; }
        public decimal? PesoTotal { get; set; }
        public string? UnidadPeso { get; set; }  // 'KG', 'LB'
        public string? MedioTransporte { get; set; } = default!; // 'AEREO','MARITIMO','TERRESTRE','MULTIMODAL'
        public string? ModalidadPagoFlete { get; set; }        // 'CONTADO', 'CREDITO', etc.
        public string? Incoterm { get; set; }                  // 'FOB','CIF', ...
        public string? NumeroAwb { get; set; }
        public string? AeropuertoEmbarqueCod { get; set; }
        public string? AeropuertoEmbarqueNombre { get; set; }
        public string? PaisDestinoIso2 { get; set; }
        public string? PaisDestinoNombre { get; set; }
        public string? AeropuertoDestinoCod { get; set; }
        public string? AeropuertoDestinoNombre { get; set; }
        public string? CodigoPostalDestino { get; set; }
        public string? NumeroPrecinto { get; set; }
        public DateTime? FechaEmbarque { get; set; }
        //Datos del transportista - F
        public string? TransportistaNombresApellidos { get; set; }
        public string? TransportistaCel { get; set; }
        public string? TransportistaDni { get; set; }
        public string? TransportistaMarca { get; set; }
        public string? TransportistaPlaca { get; set; }
        public string? NumeroBrevete { get; set; }
        public string? NumeroTarjetaPropiedad { get; set; }
        public TimeSpan? HoraIngresoDepositoTemporal { get; set; }
        public DateTime? FechaIngresoDepositoTemporal { get; set; }
        // Datos de mineral
        public string? pesoBrutoOroDore { get; set; }
        public string? purezaPlata { get; set; }
        public string? purezaOroDore { get; set; }
        public string? rateOnzOro { get; set; }
        public string? rateOnzPlata { get; set; }
        public string? rateOnzCobre { get; set; }

    }
    public class FormatoGeneradoDto
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = "";
        public string Agencia { get; set; } = "";
        public string Formato { get; set; } = "";
        public DateTime Fecha { get; set; }
        public string Ruta { get; set; } = "";
    };


}
