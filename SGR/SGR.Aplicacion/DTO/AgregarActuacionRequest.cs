using SGR.Dominio.Comun;

namespace SGR.Aplicacion.DTO;

public record class AgregarActuacionRequest (String detalle, Guid idUsuario,TipoActuacion tipo, Guid Rid);