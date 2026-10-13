
using SGR.Dominio.Comun;

namespace SGR.Aplicacion.Reclamos;

public record CambiarEstadoReclamoRequest(Guid Id, EstadoReclamo NuevoEstado, Guid IdUsuario);
