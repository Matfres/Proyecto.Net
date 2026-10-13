
namespace SGR.Aplicacion.Reclamos;

public class ObtenerReclamoPorIdUseCase
{
    private readonly IReclamoRepository _reclamoRepository;

    public ObtenerReclamoPorIdUseCase(IReclamoRepository reclamoRepository)
    {
        _reclamoRepository = reclamoRepository;
    }

    public ObtenerReclamoPorIdResponse Ejecutar(ObtenerReclamoPorIdRequest request)
    {
        var reclamo = _reclamoRepository.ObtenerPorId(request.Id);

        if (reclamo is null)
            throw new InvalidOperationException($"No se encontró el reclamo con ID {request.Id}.");

        var reclamoDTO = new ReclamoDTO(reclamo.Id, reclamo.Asunto.Valor, reclamo.FechaCreacion, reclamo.FechaUltimaModificacion, reclamo.UsuarioUltimoCambio, reclamo.Estado);

        return new ObtenerReclamoPorIdResponse(reclamoDTO);
    }
}
