
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SGR.Dominio.Actuaciones;
using SGR.Dominio.Reclamos;
using SGR.Dominio.Usuarios;

namespace SGR.Infraestructura.Configuracion;

public class SgrContext : DbContext
{
    public SgrContext(DbContextOptions<SgrContext> options) : base(options)
    {
    }

    public DbSet<Reclamo> Reclamos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Actuacion> Actuaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de objetos de valor.
        modelBuilder.Entity<Reclamo>().ComplexProperty(r => r.Asunto);
        modelBuilder.Entity<Usuario>().ComplexProperty(u => u.CorreoElectronico);
        modelBuilder.Entity<Actuacion>().ComplexProperty(a => a.detalle);

        // Correo electrónico único.
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.CorreoElectronico.Valor)
            .IsUnique();

        // Guardar los enumerados por su nombre.
        modelBuilder.Entity<Reclamo>()
            .Property(r => r.Estado)
            .HasConversion<string>();

        modelBuilder.Entity<Actuacion>()
            .Property(a => a.tipo)
            .HasConversion<string>();

        // Persistencia de la colección de permisos.
        modelBuilder.Entity<Usuario>()
            .Property<List<Permiso>>("_permisos")
            .HasConversion(
                permisos => string.Join(",", permisos),
                permisos => string.IsNullOrEmpty(permisos)
                    ? new List<Permiso>()
                    : permisos.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(Enum.Parse<Permiso>)
                        .ToList())
            .Metadata.SetValueComparer(new ValueComparer<List<Permiso>>(
                (a, b) => a!.SequenceEqual(b!),
                permisos => permisos.Aggregate(0, (hash, permiso) => HashCode.Combine(hash, permiso)),
                permisos => permisos.ToList()));
    }
}
