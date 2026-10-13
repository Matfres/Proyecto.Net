
using SGR.Aplicacion.Autorizaciones;
using SGR.Dominio.Reclamos;

namespace SGR.Aplicacion.Reclamos;

public class ModificarAsuntoReclamoUseCase
{
    private readonly IReclamoRepository _reclamoRepository;
    private readonly IAutorizacionService _autorizacionService;

    public ModificarAsuntoReclamoUseCase(IReclamoRepository reclamoRepository, IAutorizacionService autorizacionService)
    {
        _reclamoRepository = reclamoRepository;
        _autorizacionService = autorizacionService;
    }

    public ModificarAsuntoReclamoResponse Ejecutar(ModificarAsuntoReclamoRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ReclamoModificacion))
            throw new AutorizacionException("El usuario no tiene permiso para modificar el reclamo.");

        var reclamo = _reclamoRepository.ObtenerPorId(request.Id);

        if (reclamo is null)
            throw new InvalidOperationException($"No se encontró el reclamo con ID {request.Id}.");

        var nuevoAsunto = new Asunto(request.NuevoAsunto);

        reclamo.ModificarAsunto(nuevoAsunto, request.IdUsuario);

        _reclamoRepository.Modificar(reclamo);

        return new ModificarAsuntoReclamoResponse(reclamo.Id);
    }
}
