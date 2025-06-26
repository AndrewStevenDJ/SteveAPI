using Microsoft.EntityFrameworkCore;
using SteveAPI.Models;

namespace SteveAPI.Data
{
    /// <summary>
    /// DbContext principal de SteveAPI.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Tabla única de mensajes encriptados
        public DbSet<Encriptar> MensajesEncriptados => Set<Encriptar>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración extra opcional (índices, restricciones, etc.)
            modelBuilder.Entity<Encriptar>()
                        .Property(e => e.TextoCifrado)
                        .IsRequired()
                        .HasMaxLength(4096); // ajusta según necesidad

            modelBuilder.Entity<Encriptar>()
                        .HasIndex(e => e.Creado); // índice para acelerar consultas por fecha
        }
    }
}
// 