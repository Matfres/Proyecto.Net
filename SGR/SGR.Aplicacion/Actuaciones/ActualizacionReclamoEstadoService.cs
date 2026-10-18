namespace SGR.Aplicacion.Actuaciones;

using SGR.Aplicacion.Reclamos;
using SGR.Dominio.Comun;
using SGR.Dominio.Actuaciones;
public class ActualizacionEstadoReclamoService : IActualizacionEstadoReclamoService{
    private readonly IReclamoRepository _reclamoRepository;
    private readonly IActuacionRepository _actuacionRepository;

    public ActualizacionEstadoReclamoService(IReclamoRepository reclamoRepository, IActuacionRepository actuacionRepository)
    {
        _reclamoRepository = reclamoRepository;
        _actuacionRepository = actuacionRepository;
    }

    public void Ejecutar(Guid reclamoId, Guid idUsuario)
    {
        // 1. Obtener el reclamo
        var reclamo = _reclamoRepository.ObtenerPorId(reclamoId);
        if (reclamo == null)
        {
            throw new InvalidOperationException("El reclamo especificado no existe.");
        }

        // 2. Buscar las actuaciones asociadas para determinar la última por fecha
        var actuaciones = _actuacionRepository.ObtenerReclamoId(reclamoId);
        var ultimaActuacion = actuaciones.MaxBy (a => a.FechaCreacion);

        // 3. Extraer el tipo (null si no tiene actuaciones)
        TipoActuacion? ultimoTipo = ultimaActuacion?.tipo;

        // 4. Pedir a la entidad que evalúe y aplique el cambio de estado
        bool cambio = reclamo.ActualizarEstado(ultimoTipo, idUsuario);

        // 5. Solo persiste si la entidad cambió su estado efectivamente
        if (cambio)
        {
            _reclamoRepository.Modificar(reclamo);
        }
    }
}