using SGR.Aplicacion.Abstracciones;
using SGR.Infraestructura.Configuracion;

namespace SGR.Infraestructura;

public class UnidadDeTrabajo : IUnidadDeTrabajo
{
    private readonly SgrContext _context;

    public UnidadDeTrabajo(SgrContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void GuardarCambios()
    {
        // EF Core agrupa automáticamente todas las operaciones de inserción, 
        // modificación y eliminación (Change Tracker) y las ejecuta en 
        // una única transacción atómica en la base de datos.
        _context.SaveChanges();
    }
}

