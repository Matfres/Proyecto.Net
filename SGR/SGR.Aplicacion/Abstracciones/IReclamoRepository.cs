using SGR.Dominio.Comun;
using SGR.Dominio.Reclamos;

namespace SGR.Aplicacion.Abstracciones;

public interface IReclamoRepository : IRepository<Reclamo>
{    
    void Modificar(Reclamo unReclamo);
    IEnumerable<Reclamo> ObtenerPorEstado (EstadoReclamo estado);
}
