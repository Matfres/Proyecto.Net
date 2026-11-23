
using SGR.Dominio.Comun;

namespace SGR.Aplicacion.DTO;

public record ActuacionDTO(
    Guid Id,
    Guid ReclamoId,
    string Detalle,
    DateTime FechaCreacion,
    DateTime FechaUltimaModificacion,
    Guid UsuarioUltimoCambio,
    TipoActuacion Tipo
);
