namespace SGR.Aplicacion.Abstracciones;
public interface IRepository<T>
{
    void Agregar(T entidad);

    T? ObtenerPorId(Guid id);

    IEnumerable<T> ObtenerTodos();
    
    void Eliminar(Guid id);
    
}
