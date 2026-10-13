
namespace SGR.Aplicacion.Reclamos;

public record ModificarAsuntoReclamoRequest(Guid Id, string NuevoAsunto, Guid IdUsuario);
