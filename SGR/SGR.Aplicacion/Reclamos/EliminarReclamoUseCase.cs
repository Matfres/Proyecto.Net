
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Usuarios;
using SGR.Aplicacion.DTO;
namespace SGR.Aplicacion.Reclamos;

public class EliminarReclamoUseCase
{
    private readonly IReclamoRepository _reclamoRepository;
    private readonly IAutorizacionService _autorizacionService;
    private readonly IActuacionRepository _actuacionRepository;
    private readonly IUnidadDeTrabajo _uow;
    public EliminarReclamoUseCase(IReclamoRepository reclamoRepository,  IActuacionRepository actuacionRepository, IAutorizacionService autorizacionService,IUnidadDeTrabajo uow)
    {
        _uow= uow;
        _reclamoRepository = reclamoRepository;
        _actuacionRepository = actuacionRepository;
        _autorizacionService = autorizacionService;
    }

    public EliminarReclamoResponse Ejecutar(EliminarReclamoRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ReclamoBaja))
            throw new AutorizacionException("El usuario no tiene permiso para eliminar reclamos.");

        var reclamo = _reclamoRepository.ObtenerPorId(request.Id);

        if (reclamo is null)
            throw new EntidadNoEncontradaException($"No se encontró el reclamo con ID {request.Id}.");
        
        var actuaciones = _actuacionRepository.ObtenerReclamoId(reclamo.Id).ToList();
        
        foreach (var actuacion in actuaciones)
        {
            _actuacionRepository.Eliminar(actuacion.Id);
        }

        _reclamoRepository.Eliminar(reclamo.Id);
        _uow.GuardarCambios();
        return new EliminarReclamoResponse(reclamo.Id);
    }
}
