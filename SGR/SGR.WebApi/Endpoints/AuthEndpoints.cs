
using SGR.Aplicacion.DTO;
using SGR.Aplicacion.Usuarios;

namespace SGR.WebApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var grupo = endpoints.MapGroup("/auth")
            .WithTags("Autenticación");

        grupo.MapPost("/login", (LoginBody body, LoginUseCase casoDeUso) =>
        {
            var request = new LoginRequest(body.Correo, body.Password);
            var response = casoDeUso.Ejecutar(request);
            return Results.Ok(response);
        });

        grupo.MapPost("/registro", (RegistrarUsuarioBody body, RegistrarUsuarioUseCase casoDeUso) =>
        {
            var request = new RegistrarUsuarioRequest(body.Correo, body.Nombre, body.Password);
            var response = casoDeUso.Ejecutar(request);
            return Results.Created("/auth/registro", response);
        });
    }
}

public record LoginBody(string Correo, string Password);

public record RegistrarUsuarioBody(string Correo, string Nombre, string Password);
