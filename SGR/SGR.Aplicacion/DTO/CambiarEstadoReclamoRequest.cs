
using SGR.Dominio.Comun;

namespace SGR.Aplicacion.DTO;

public record CambiarEstadoReclamoRequest(Guid Id, EstadoReclamo NuevoEstado, Guid IdUsuario);
