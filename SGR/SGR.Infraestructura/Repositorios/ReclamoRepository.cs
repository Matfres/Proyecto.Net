using SGR.Aplicacion.Abstracciones;
using SGR.Dominio.Comun;
using SGR.Dominio.Reclamos;
using SGR.Infraestructura.Configuracion;

namespace SGR.Infraestructura.Repositorios;

public class ReclamoRepository: Repository<Reclamo>, IReclamoRepository{
     public ReclamoRepository(SgrContext context) : base(context) { }

 public void Modificar (Reclamo actuacion)
    {
        _context.Reclamos.Update(actuacion);
    }
    public IEnumerable<Reclamo> ObtenerPorEstado (EstadoReclamo estado)
    {
        return _context.Reclamos
        .Where(r => r.Estado == estado)
        .ToList();
    }
}