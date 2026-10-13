
namespace SGR.Aplicacion.Reclamos;

public class ListarReclamosUseCase
{
    private readonly IReclamoRepository _reclamoRepository;

    public ListarReclamosUseCase(IReclamoRepository reclamoRepository)
    {
        _reclamoRepository = reclamoRepository;
    }

    public ListarReclamosResponse Ejecutar(ListarReclamosRequest request)
    {
        var reclamos = _reclamoRepository.ObtenerTodos();

        var reclamosDTO = reclamos.Select(reclamo => new ReclamoDTO(reclamo.Id, reclamo.Asunto.Valor, reclamo.FechaCreacion, reclamo.FechaUltimaModificacion, reclamo.UsuarioUltimoCambio, reclamo.Estado));

        return new ListarReclamosResponse(reclamosDTO);
    }
}
