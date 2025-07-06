using Microsoft.EntityFrameworkCore;

namespace BackendFormatos.Data
{
    public class DbFormatoContext : DbContext
    {
        public DbFormatoContext(DbContextOptions<DbFormatoContext> options) : base(options) { }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region TablasMaestras
            #endregion

            #region Store Procedure Query Select
            #endregion
        }
    }
}
