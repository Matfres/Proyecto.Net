namespace SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Usuarios;
using SGR.Dominio.Actuaciones;
using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.DTO;

public class ModificarActuacionUseCase
{
    private readonly IActuacionRepository _actuacionRepository;
    private readonly IAutorizacionService _autorizacionService;
    private readonly IActualizacionEstadoReclamoService _actualizacionEstadoReclamoService;
    private readonly IUnidadDeTrabajo _uow;

    public ModificarActuacionUseCase(IActuacionRepository actuacionRepository, IAutorizacionService autorizacionService, IActualizacionEstadoReclamoService actualizacionEstadoReclamoService,IUnidadDeTrabajo uow)
    {
        _actuacionRepository=actuacionRepository;
        _actualizacionEstadoReclamoService=actualizacionEstadoReclamoService;
        _uow = uow;
        _autorizacionService=autorizacionService;
    }

    public ModificarActuacionResponse Ejecutar (ModificarActuacionRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.idUsuario, Permiso.ActuacionModificacion))
        {
            throw new AutorizacionException ($"el usuario no tiene permiso para modificar la actuacion de ID {request.id}");
        }
        var actuacion= _actuacionRepository.ObtenerPorId(request.id);
        if (actuacion == null)
        {
            throw new EntidadNoEncontradaException("no se encontro ninguna actuacion con ese ID");
        }
        actuacion.ModificarDetalle(new Detalle(request.detalle),request.idUsuario);
        actuacion.CambiarTipo(request.tipo,request.idUsuario);
        _actuacionRepository.Modificar(actuacion);
        _actualizacionEstadoReclamoService.Ejecutar(actuacion.ReclamoId,request.idUsuario);
        _uow.GuardarCambios();
        return new ModificarActuacionResponse(actuacion.Id);
    }
}
