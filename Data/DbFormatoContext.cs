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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region TablasMaestras
            modelBuilder.Entity<Clientes>().ToTable("Clientes");
            modelBuilder.Entity<Exportadores>().ToTable("Exportadores");
            modelBuilder.Entity<Agencias>().ToTable("Agencias");
            #endregion

            #region Store Procedure Query Select
            #endregion
        }
    }
}
