namespace SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.DTO;

public class ListarActuacionPorReclamoUseCase
{
    private readonly IActuacionRepository _actuacionRepository;

    public ListarActuacionPorReclamoUseCase ( IActuacionRepository actuacionRepository)
    {
        _actuacionRepository=actuacionRepository;
    }
    public ListarActuacionPorReclamoResponse Ejecutar (ListarActuacionPorReclamoRequest request)
    {
        var actuaciones = _actuacionRepository.ObtenerReclamoId(request.reclamoId);
        var actuacionesDTO = actuaciones.Select(actuacion => new ActuacionDTO(actuacion.Id,
        actuacion.ReclamoId,
        actuacion.detalle.ToString(),
        actuacion.FechaCreacion,
        actuacion.FechaUltimaModificacion,
        actuacion.UsuarioUltimoCambio,
        actuacion.tipo)); 
        return new ListarActuacionPorReclamoResponse( actuacionesDTO);
    }
}