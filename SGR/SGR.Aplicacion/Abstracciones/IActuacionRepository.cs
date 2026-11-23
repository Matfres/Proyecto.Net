using SGR.Dominio.Actuaciones;
namespace SGR.Aplicacion.Abstracciones;

public interface IActuacionRepository : IRepository<Actuacion>
{
    IEnumerable<Actuacion> ObtenerReclamoId(Guid idReclamo);
    void Modificar(Actuacion unaActuacion);
}