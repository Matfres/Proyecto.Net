
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Actuaciones;
namespace SGR.Aplicacion.Reclamos;

public class EliminarReclamoUseCase
{
    private readonly IReclamoRepository _reclamoRepository;
    private readonly IAutorizacionService _autorizacionService;
    private readonly IActuacionRepository _actuacionRepository;

    public EliminarReclamoUseCase(IReclamoRepository reclamoRepository,  IActuacionRepository actuacionRepository, IAutorizacionService autorizacionService)
    {
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
            throw new InvalidOperationException($"No se encontró el reclamo con ID {request.Id}.");
        
        var actuaciones = _actuacionRepository.ObtenerReclamoId(reclamo.Id).ToList();
        
        foreach (var actuacion in actuaciones)
        {
            _actuacionRepository.Eliminar(actuacion.id);
        }

        _reclamoRepository.Eliminar(reclamo.Id);

        return new EliminarReclamoResponse(reclamo.Id);
    }
}
