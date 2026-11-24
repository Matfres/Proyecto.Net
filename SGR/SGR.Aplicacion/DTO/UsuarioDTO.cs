using SGR.Dominio.Usuarios;

namespace SGR.Aplicacion.DTO;

public record UsuarioDTO(
    Guid Id,
    string Correo,
    string Nombre,
    bool EsAdministrador,
    IEnumerable<Permiso> Permisos
);