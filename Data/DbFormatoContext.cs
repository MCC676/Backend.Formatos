using BackendFormatos.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendFormatos.Data
{
    public class DbFormatoContext : DbContext
    {
        public DbFormatoContext(DbContextOptions<DbFormatoContext> options) : base(options) { }

        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Exportadores> Exportadores { get; set; }
        public DbSet<Agencias> Agencias { get; set; }
        public DbSet<DocumentoTransportista> DocumentoTransportistas { get; set; }
        public DbSet<DocumentoCargaExportar> DocumentoCargaExportar { get; set; }
        public DbSet<DocumentoParte> DocumentoParte { get; set; }
        public DbSet<AgenciaFormatos> AgenciaFormatos { get; set; }
        public DbSet<ProveedorFormatos> ProveedorFormatos { get; set; }
        public DbSet<FormatoGenerado> FormatosGenerados => Set<FormatoGenerado>();

        // NUEVOS (Esquema normalizado de Proveedores)
        public DbSet<TipoProveedor> TipoProveedor => Set<TipoProveedor>();
        public DbSet<Proveedor> Proveedor => Set<Proveedor>();
        public DbSet<ProveedorTransporte> ProveedorTransporte => Set<ProveedorTransporte>();
        public DbSet<ProveedorSeguridad> ProveedorSeguridad => Set<ProveedorSeguridad>();
        public DbSet<ProveedorBanco> ProveedorBanco => Set<ProveedorBanco>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region TablasMaestras
            modelBuilder.Entity<Clientes>().ToTable("Clientes");
            modelBuilder.Entity<Exportadores>().ToTable("Exportadores");
            modelBuilder.Entity<Agencias>().ToTable("Agencias");
            modelBuilder.Entity<AgenciaFormatos>().ToTable("AgenciaFormatos");
            modelBuilder.Entity<ProveedorFormatos>().ToTable("ProveedorFormatos");
            modelBuilder.Entity<FormatoGenerado>().ToTable("FormatoGenerado");
            modelBuilder.Entity<DocumentoParte>().ToTable("DocumentoParte");
            modelBuilder.Entity<DocumentoTransportista>().ToTable("DocumentoTransportista");
            modelBuilder.Entity<DocumentoCargaExportar>().ToTable("DocumentoCargaExportar");
            #endregion

            #region Proveedores (nuevo)

            // Alternativa rápida (si aún no tienes las config classes):
            modelBuilder.Entity<TipoProveedor>().ToTable("TipoProveedor").HasKey(x => x.TipoProveedorId);
            modelBuilder.Entity<Proveedor>().ToTable("Proveedor");
            modelBuilder.Entity<ProveedorTransporte>().ToTable("ProveedorTransporte");
            modelBuilder.Entity<ProveedorSeguridad>().ToTable("ProveedorSeguridad").HasKey(x => x.ProveedorId);
            modelBuilder.Entity<ProveedorBanco>().ToTable("ProveedorBanco");
            #endregion

            #region Store Procedure Query Select
            #endregion
        }
    }
}
