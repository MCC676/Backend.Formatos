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
        public DbSet<FormatoGenerado> FormatosGenerados => Set<FormatoGenerado>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region TablasMaestras
            modelBuilder.Entity<Clientes>().ToTable("Clientes");
            modelBuilder.Entity<Exportadores>().ToTable("Exportadores");
            modelBuilder.Entity<Agencias>().ToTable("Agencias");
            modelBuilder.Entity<AgenciaFormatos>().ToTable("AgenciaFormatos");
            modelBuilder.Entity<FormatoGenerado>().ToTable("FormatoGenerado");
            modelBuilder.Entity<DocumentoParte>().ToTable("DocumentoParte");
            modelBuilder.Entity<DocumentoTransportista>().ToTable("DocumentoTransportista");
            modelBuilder.Entity<DocumentoCargaExportar>().ToTable("DocumentoCargaExportar");
            #endregion

            #region Store Procedure Query Select
            #endregion
        }
    }
}
