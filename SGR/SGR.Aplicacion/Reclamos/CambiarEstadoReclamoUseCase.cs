
using SGR.Aplicacion.Autorizaciones;

namespace SGR.Aplicacion.Reclamos;

public class CambiarEstadoReclamoUseCase
{
    private readonly IReclamoRepository _reclamoRepository;
    private readonly IAutorizacionService _autorizacionService;

    public CambiarEstadoReclamoUseCase(IReclamoRepository reclamoRepository, IAutorizacionService autorizacionService)
    {
        _reclamoRepository = reclamoRepository;
        _autorizacionService = autorizacionService;
    }

    public CambiarEstadoReclamoResponse Ejecutar(CambiarEstadoReclamoRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ReclamoModificacion))
            throw new AutorizacionException("El usuario no tiene permiso para cambiar el estado de reclamos.");

        var reclamo = _reclamoRepository.ObtenerPorId(request.Id);

        if (reclamo is null)
            throw new InvalidOperationException($"No se encontró el reclamo con ID {request.Id}.");

        reclamo.CambiarEstado(request.NuevoEstado, request.IdUsuario);

        _reclamoRepository.Modificar(reclamo);

        return new CambiarEstadoReclamoResponse(reclamo.Id);
    }
}
