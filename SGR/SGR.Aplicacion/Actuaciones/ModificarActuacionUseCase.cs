namespace SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Reclamos;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Usuarios;
using SGR.Dominio.Actuaciones;

public class ModificarActuacionUseCase
{
    private readonly IActuacionRepository _actuacionRepository;
    private readonly IAutorizacionService _autorizacionService;
    private readonly IActualizacionEstadoReclamoService _actualizacionEstadoReclamoService;


    public ModificarActuacionUseCase(IActuacionRepository actuacionRepository, IAutorizacionService autorizacionService, IActualizacionEstadoReclamoService actualizacionEstadoReclamoService)
    {
        _actuacionRepository=actuacionRepository;
        _actualizacionEstadoReclamoService=actualizacionEstadoReclamoService;
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
        return new ModificarActuacionResponse(actuacion.id);
    }
}
