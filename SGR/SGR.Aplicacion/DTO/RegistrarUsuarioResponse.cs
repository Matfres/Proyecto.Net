public record RegistrarUsuarioResponse(
    Guid Id,
    string Correo,
    string Nombre,
    bool EsAdministrador
);