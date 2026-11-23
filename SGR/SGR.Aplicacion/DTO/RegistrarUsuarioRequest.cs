namespace SGR.Aplicacion.DTO;

public record RegistrarUsuarioRequest(
    string Correo,
    string Nombre,
    string Password
);