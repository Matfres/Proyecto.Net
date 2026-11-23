
using SGR.Dominio.Comun;

namespace SGR.Aplicacion.DTO;

public record ReclamoDTO(
    Guid Id,
    string Asunto,
    DateTime FechaCreacion,
    DateTime FechaUltimaModificacion,
    Guid UsuarioUltimoCambio,
    EstadoReclamo Estado
);
