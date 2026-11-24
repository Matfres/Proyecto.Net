namespace SGR.Aplicacion.DTO;

public record ModificarMisDatosRequest(
    string? NuevoNombre,
    string? PasswordActual,
    string? NuevaPassword
);