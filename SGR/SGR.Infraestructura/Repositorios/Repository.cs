using SGR.Aplicacion.Abstracciones;
using SGR.Dominio;
using SGR.Infraestructura.Configuracion;
using SGR.Infraestructura.Comun;

namespace SGR.Infraestructura.Repositorios;

public class Repository<T> : IRepository<T> where T : Entidad
{
    // Usamos readonly para proteger la instancia
    protected readonly SgrContext _context;

    public Repository(SgrContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Agregar(T entidad)
    {
        if (entidad == null)
            throw new ArgumentNullException(nameof(entidad), "La entidad no puede ser nula");

        // NOTA SOBRE _context.Set<T>():
        // En un DbContext tradicional accedíamos explícitamente a cada tabla, 
        // por ejemplo: _context.Alumnos.Add(entidad);
        // Sin embargo, en un Repositorio Genérico no sabemos de antemano si 'T' 
        // representa a un Alumno, un Examen o cualquier otra entidad.
        // El método Set<T>() es la solución de Entity Framework para esto: 
        // busca dinámicamente en el contexto y devuelve el DbSet<> correspondiente 
        // al tipo 'T' que se esté utilizando en ese momento.
        _context.Set<T>().Add(entidad);
    }
    public void Eliminar(Guid id)
    {
        var entidad = _context.Set<T>().Find(id);
        if (entidad == null)
            throw new RepositorioException($"No se encontró la entidad con ID: {id} para eliminar");

        _context.Set<T>().Remove(entidad);
    }

    public T? ObtenerPorId(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("El ID no puede ser vacío", nameof(id));

        return _context.Set<T>().Find(id);
    }

    public IEnumerable<T> ObtenerTodos()
    {
        return _context.Set<T>().ToList(); // O retornar como IEnumerable directo
    }
}