
namespace SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Reclamos;
using SGR.Aplicacion.Autorizaciones; 
using SGR.Aplicacion.Comun;
using SGR.Dominio.Usuarios;
using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.DTO;
public class  EliminarActuacionUseCase{

private readonly IActuacionRepository _actuacionRepository;
private readonly IReclamoRepository _reclamoRepository;
private readonly IActualizacionEstadoReclamoService _actualizacionEstadoReclamoService;
private readonly IAutorizacionService _autorizacionService;

private readonly IUnidadDeTrabajo _uow;
public EliminarActuacionUseCase(IActuacionRepository actuacionRepository, IReclamoRepository reclamoRepository, IActualizacionEstadoReclamoService actualizacionEstadoReclamoService, IAutorizacionService autorizacionService, IUnidadDeTrabajo uow)
    {
        _actuacionRepository= actuacionRepository;
        _reclamoRepository = reclamoRepository;
        _actualizacionEstadoReclamoService = actualizacionEstadoReclamoService;
        _autorizacionService = autorizacionService;
        _uow = uow;
    }

    public EliminarActuacionResponse Ejecutar (EliminarActuacionRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ActuacionBaja))
        {
            throw new AutorizacionException("El usuario no tiene permiso para dar de baja la actuacion");  }
        var actuacion = _actuacionRepository.ObtenerPorId(request.Id);
        if ( actuacion == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el actuacion con ID {request.Id}.");
        }
        _actuacionRepository.Eliminar(request.Id);
        _actualizacionEstadoReclamoService.Ejecutar(actuacion.ReclamoId,request.IdUsuario);
        _uow.GuardarCambios();
        return new EliminarActuacionResponse(actuacion.Id);
    }
}