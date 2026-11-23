using SGR.Dominio.Actuaciones;
using SGR.Dominio.Comun;

namespace SGR.Aplicacion.DTO;

public record class ModificarActuacionRequest(Guid id, Guid idUsuario,TipoActuacion tipo, String detalle);