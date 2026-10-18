using SGR.Dominio.Actuaciones;
using SGR.Dominio.Comun;

namespace SGR.Aplicacion.Actuaciones;

public record class ModificarActuacionRequest(Guid id, Guid idUsuario,TipoActuacion tipo, Detalle detalle);