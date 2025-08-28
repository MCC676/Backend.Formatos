using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BackendFormatos.Models
{
    public class DocumentoParte
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public string? Rol { get; set; }
        public string? RazonSocial { get; set; }
        public string? NombresApellidos { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? EstadoProvincia { get; set; }
        public string? Pais { get; set; }
        public string? ZipCode { get; set; }
        public string? Celular { get; set; }
        public string? Email { get; set; }
        public string? Contacto { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime? CreateDate { get; set; }
    }

    public class DocumentoCargaExportar
    {
        public int Id { get; set; }
        public string? DescripcionProducto { get; set; }
        public int? NumeroBultos { get; set; }
        public decimal? PesoTotal { get; set; }
        public string? UnidadPeso { get; set; }  // 'KG', 'LB'
        public string MedioTransporte { get; set; } = default!; // 'AEREO','MARITIMO','TERRESTRE','MULTIMODAL'
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
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    }

    public class DocumentoTransportista
    {
        public int Id { get; set; }
        public string? NombresApellidos { get; set; }
        public string? Celular { get; set; }
        public string? NumeroDni { get; set; }
        public string? VehiculoMarca { get; set; }
        public string? VehiculoPlaca { get; set; }
        public string? NumeroBrevete { get; set; }
        public string? NumeroTarjetaPropiedad { get; set; }
        public TimeSpan? HoraIngresoDepositoTemporal { get; set; }
        public DateTime? FechaIngresoDepositoTemporal { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    }
}
