using SGR.Aplicacion.Autorizaciones;
using SGR.Dominio.Actuaciones;
using SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Reclamos;
namespace SGR.Aplicacion.Actuaciones;

public class AgregarActuacionUseCase
{
    private readonly IActuacionRepository _actuacionRepository;
    private readonly IAutorizacionService _autorizacionService;
    private readonly IReclamoRepository _reclamoRepository;

    private readonly IActualizacionEstadoReclamoService _actualizacionReclamoEstadoService;
    
    public AgregarActuacionUseCase(IActuacionRepository actuacionRepository, IAutorizacionService autorizacionService,IReclamoRepository reclamoRepository, IActualizacionEstadoReclamoService actualizacionEstadoReclamoService)
    {
        _actuacionRepository = actuacionRepository;
        _autorizacionService = autorizacionService;
        _reclamoRepository = reclamoRepository;
        _actualizacionReclamoEstadoService = actualizacionEstadoReclamoService;
    }

    public AgregarActuacionResponse Ejecutar(AgregarActuacionRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.idUsuario, Permiso.ActuacionAlta)){
            throw new AutorizacionException("El usuario no tiene permiso para dar de alta la actuacion");
    }
    if (_reclamoRepository.ObtenerPorId(request.Rid) == null)
        {
            throw new InvalidOperationException($"No se encontró el reclamo con ID {request.Rid}.");
        }
        var detalle = new Detalle (request.detalle);
        var actuacion = new Actuacion(request.Rid,request.tipo,detalle,DateTime.Now, request.idUsuario);

        _actuacionRepository.Agregar(actuacion);
        _actualizacionReclamoEstadoService.Ejecutar(request.Rid, request.idUsuario);

        return new AgregarActuacionResponse(actuacion.id);
    }
}
