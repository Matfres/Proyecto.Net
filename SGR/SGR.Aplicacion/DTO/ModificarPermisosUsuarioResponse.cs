using SGR.Dominio.Usuarios;
namespace SGR.Aplicacion.DTO;
public record ModificarPermisosUsuarioResponse(
    Guid Id,
    string Correo,
    string Nombre,
    IEnumerable<Permiso> Permisos
);