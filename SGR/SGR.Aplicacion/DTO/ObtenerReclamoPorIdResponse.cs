
namespace SGR.Aplicacion.DTO;

public record ObtenerReclamoPorIdResponse(ReclamoDTO Reclamo, IEnumerable<ActuacionDTO> Actuaciones);
