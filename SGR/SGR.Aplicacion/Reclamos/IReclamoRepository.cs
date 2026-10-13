using SGR.Dominio.Reclamos;

namespace SGR.Aplicacion.Reclamos;

public interface IReclamoRepository
{
    void Agregar(Reclamo unReclamo);
    Reclamo? ObtenerPorId(Guid id);
    void Modificar(Reclamo unReclamo);
    void Eliminar(Guid id);
    IEnumerable<Reclamo> ObtenerTodos();
}