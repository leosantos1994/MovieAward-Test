using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.DB
{
    public class MSSQLContext : DbContext
    {
        public MSSQLContext(DbContextOptions<MSSQLContext> options) : base(options) { }

        public DbSet<Movie> Movie { get; set; }

        //Apenas uma instancia não será preciso habilitar o lazyloading

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //                                         => optionsBuilder.UseLazyLoadingProxies();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Movie>().ToTable("Movie");
            //Apenas uma instancia não será necessário criar o map
        }
    }
}
