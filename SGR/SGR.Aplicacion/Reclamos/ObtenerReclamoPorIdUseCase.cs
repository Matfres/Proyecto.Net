using SGR.Aplicacion.Comun;
using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.DTO;

namespace SGR.Aplicacion.Reclamos;


public class ObtenerReclamoPorIdUseCase
{
    private readonly IReclamoRepository _reclamoRepository;
    private readonly IActuacionRepository _ActuacionesRepository;

    public ObtenerReclamoPorIdUseCase(IReclamoRepository reclamoRepository,IActuacionRepository actuacionRepository)
    {
        _reclamoRepository = reclamoRepository;
        _ActuacionesRepository = actuacionRepository;
    }

    public ObtenerReclamoPorIdResponse Ejecutar(ObtenerReclamoPorIdRequest request)
    {
        var reclamo = _reclamoRepository.ObtenerPorId(request.Id);

        if (reclamo is null)
            throw new EntidadNoEncontradaException($"No se encontró el reclamo con ID {request.Id}.");

        var reclamoDTO = new ReclamoDTO(reclamo.Id, reclamo.Asunto.Valor, reclamo.FechaCreacion, reclamo.FechaUltimaModificacion, reclamo.UsuarioUltimoCambio, reclamo.Estado);
        var actuaciones = _ActuacionesRepository.ObtenerReclamoId(reclamo.Id);
        var actuacionesDTO = actuaciones.Select(a => new ActuacionDTO ( a.Id,a.ReclamoId,a.detalle.Cuerpo,a.FechaCreacion,a.FechaUltimaModificacion,a.UsuarioUltimoCambio,a.tipo));

        return new ObtenerReclamoPorIdResponse(reclamoDTO,actuacionesDTO);
    }
}
