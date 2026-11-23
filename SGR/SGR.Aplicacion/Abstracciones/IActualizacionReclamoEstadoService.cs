namespace SGR.Aplicacion.Abstracciones;

public interface IActualizacionEstadoReclamoService
{
    void Ejecutar(Guid reclamoId, Guid idUsuario);
    }