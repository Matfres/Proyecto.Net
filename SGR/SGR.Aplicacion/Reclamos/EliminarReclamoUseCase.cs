
using SGR.Aplicacion.Autorizaciones;

namespace SGR.Aplicacion.Reclamos;

public class EliminarReclamoUseCase
{
    private readonly IReclamoRepository _reclamoRepository;
    private readonly IAutorizacionService _autorizacionService;

    public EliminarReclamoUseCase(IReclamoRepository reclamoRepository, IAutorizacionService autorizacionService)
    {
        _reclamoRepository = reclamoRepository;
        _autorizacionService = autorizacionService;
    }

    public EliminarReclamoResponse Ejecutar(EliminarReclamoRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ReclamoBaja))
            throw new AutorizacionException("El usuario no tiene permiso para eliminar reclamos.");

        var reclamo = _reclamoRepository.ObtenerPorId(request.Id);

        if (reclamo is null)
            throw new InvalidOperationException($"No se encontró el reclamo con ID {request.Id}.");

        _reclamoRepository.Eliminar(reclamo.Id);

        return new EliminarReclamoResponse(reclamo.Id);
    }
}
