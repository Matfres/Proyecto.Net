using SGR.Dominio.Actuaciones;

namespace SGR.Aplicacion.Actuaciones;

public interface IActuacionRepository
{
    void Agregar(Actuacion unaActuacion);
    Actuacion? ObtenerPorId(Guid id);
    IEnumerable<Actuacion> ObtenerReclamoId(Guid idReclamo);
    
    void Modificar(Actuacion unaActuacion);
    void Eliminar(Guid id);
    IEnumerable<Actuacion> ObtenerTodos();
}