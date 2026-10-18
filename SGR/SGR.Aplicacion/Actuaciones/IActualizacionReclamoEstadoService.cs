namespace SGR.Aplicacion.Actuaciones;

public interface IActualizacionEstadoReclamoService
{
    void Ejecutar(Guid reclamoId, Guid idUsuario);
    }