using SGR.Aplicacion.Abstracciones;
using SGR.Infraestructura.Configuracion;
using SGR.Dominio.Actuaciones;
using Microsoft.EntityFrameworkCore;

namespace SGR.Infraestructura.Repositorios;

public class ActuacionRepository : Repository<Actuacion>, IActuacionRepository
{
      public ActuacionRepository(SgrContext context) : base(context) { }

      public IEnumerable<Actuacion> ObtenerReclamoId( Guid reclamoId)
    {
        // 1. Trae desde SQLite las actuaciones de ese reclamo y las mete en la memoria (.Local) del contexto
    _context.Actuaciones
        .Where(a => a.ReclamoId == reclamoId)
        .Load();

    // 2. Consulta la memoria (.Local): ya incluye las que hiciste .Add(), 
    // ya excluye las que hiciste .Remove() y ya tiene los cambios de las modificadas
    return _context.Actuaciones.Local
        .Where(a => a.ReclamoId == reclamoId)
        .ToList();

}
    public void Modificar (Actuacion actuacion)
    {
        _context.Actuaciones.Update(actuacion);
    }
    }


