namespace SGR.Aplicacion.DTO;
public record ListarUsuariosResponse(
    IEnumerable<UsuarioDTO> Usuarios
);