
using SGR.Aplicacion.Autorizaciones;
using SGR.Dominio.Reclamos;

namespace SGR.Aplicacion.Reclamos;

public class AgregarReclamoUseCase
{
    private readonly IReclamoRepository _reclamoRepository;
    private readonly IAutorizacionService _autorizacionService;

    public AgregarReclamoUseCase(IReclamoRepository reclamoRepository, IAutorizacionService autorizacionService)
    {
        _reclamoRepository = reclamoRepository;
        _autorizacionService = autorizacionService;
    }

    public AgregarReclamoResponse Ejecutar(AgregarReclamoRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ReclamoAlta))
            throw new AutorizacionException("El usuario no tiene permiso para dar de alta el reclamo");

        var asunto = new Asunto(request.Asunto);
        var reclamo = new Reclamo(asunto, DateTime.Now, request.IdUsuario);

        _reclamoRepository.Agregar(reclamo);

        return new AgregarReclamoResponse(reclamo.Id);
    }
}
