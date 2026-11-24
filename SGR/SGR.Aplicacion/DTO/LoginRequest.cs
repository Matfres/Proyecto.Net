namespace SGR.Aplicacion.DTO;

public record LoginRequest(
    string Correo,
    string Password
);