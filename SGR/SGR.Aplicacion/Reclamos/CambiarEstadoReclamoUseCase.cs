
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Usuarios;
using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.DTO;
namespace SGR.Aplicacion.Reclamos;

public class CambiarEstadoReclamoUseCase
{
    private readonly IReclamoRepository _reclamoRepository;
    private readonly IAutorizacionService _autorizacionService;
    private readonly IUnidadDeTrabajo _uow;
    public CambiarEstadoReclamoUseCase(IReclamoRepository reclamoRepository, IAutorizacionService autorizacionService,IUnidadDeTrabajo uow)
    {
        _reclamoRepository = reclamoRepository;
        _uow=uow;
        _autorizacionService = autorizacionService;
    }

    public CambiarEstadoReclamoResponse Ejecutar(CambiarEstadoReclamoRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ReclamoModificacion))
            throw new AutorizacionException("El usuario no tiene permiso para cambiar el estado de reclamos.");

        var reclamo = _reclamoRepository.ObtenerPorId(request.Id);

        if (reclamo is null)
            throw new EntidadNoEncontradaException($"No se encontró el reclamo con ID {request.Id}.");

        reclamo.CambiarEstado(request.NuevoEstado, request.IdUsuario);

        //_reclamoRepository.Modificar(reclamo); es redundante: Aca es Guardar() no GuardadCambios() ya persiste las modificaciones detectadas por EF Core
        _uow.GuardarCambios();

        return new CambiarEstadoReclamoResponse(reclamo.Id);
    }
}
