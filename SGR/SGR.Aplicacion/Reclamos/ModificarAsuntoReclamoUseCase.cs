
using SGR.Aplicacion.Autorizaciones;
using SGR.Dominio.Reclamos;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Usuarios;
using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.DTO;

namespace SGR.Aplicacion.Reclamos;

public class ModificarAsuntoReclamoUseCase
{
    private readonly IReclamoRepository _reclamoRepository;
    private readonly IAutorizacionService _autorizacionService;
    private readonly IUnidadDeTrabajo _uow;
    public ModificarAsuntoReclamoUseCase(IReclamoRepository reclamoRepository, IAutorizacionService autorizacionService, IUnidadDeTrabajo uow)
    {
        _uow= uow;
        _reclamoRepository = reclamoRepository;
        _autorizacionService = autorizacionService;
    }

    public ModificarAsuntoReclamoResponse Ejecutar(ModificarAsuntoReclamoRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ReclamoModificacion))
            throw new AutorizacionException("El usuario no tiene permiso para modificar el reclamo.");

        var reclamo = _reclamoRepository.ObtenerPorId(request.Id);

        if (reclamo is null)
            throw new EntidadNoEncontradaException($"No se encontró el reclamo con ID {request.Id}.");

        var nuevoAsunto = new Asunto(request.NuevoAsunto);

        reclamo.ModificarAsunto(nuevoAsunto, request.IdUsuario);

        //_reclamoRepository.Modificar(reclamo); llamar a _reclamoRepository.Modificar(reclamo) es redundante: Aca es Guardar() no GuardadCambios() ya persiste las modificaciones detectadas por EF Core
        _uow.GuardarCambios();
        return new ModificarAsuntoReclamoResponse(reclamo.Id);
    }
}
