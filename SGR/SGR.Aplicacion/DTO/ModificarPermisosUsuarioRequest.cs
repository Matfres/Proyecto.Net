using SGR.Dominio.Usuarios;

namespace SGR.Aplicacion.DTO;

public record ModificarPermisosUsuarioRequest(
    Guid IdUsuario,
    IEnumerable<Permiso> Permisos
);