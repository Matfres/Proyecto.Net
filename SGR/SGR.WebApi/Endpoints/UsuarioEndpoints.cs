
using System.Security.Claims;
using SGR.Aplicacion.DTO;
using SGR.Aplicacion.Usuarios;

namespace SGR.WebApi.Endpoints;

public static class UsuarioEndpoints
{
    public static void MapUsuarioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var grupo = endpoints.MapGroup("/usuarios")
            .RequireAuthorization()
            .WithTags("Usuarios");

        // Modificar los datos del usuario autenticado.
        grupo.MapPut("/yo", (ModificarMisDatosBody body, ClaimsPrincipal usuario, ModificarMisDatosUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            var request = new ModificarMisDatosRequest(body.NuevoNombre, body.PasswordActual, body.NuevaPassword);
            var response = casoDeUso.Ejecutar(request, idUsuario);

            return Results.Ok(response);
        });

        // Listar todos los usuarios (solo administradores).
        grupo.MapGet("/", (ClaimsPrincipal usuario, ListarUsuariosUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            var response = casoDeUso.Ejecutar(idUsuario);

            return Results.Ok(response);
        });

        // Eliminar un usuario (solo administradores).
        grupo.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal usuario, EliminarUsuarioUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuarioEjecutor))
                return Results.Unauthorized();

            var request = new EliminarUsuarioRequest(id);
            var response = casoDeUso.Ejecutar(request, idUsuarioEjecutor);

            return Results.Ok(response);
        });

        // Modificar los permisos de un usuario (solo administradores).
        grupo.MapPut("/{id:guid}/permisos", (Guid id, ModificarPermisosUsuarioBody body, ClaimsPrincipal usuario, ModificarPermisosUsuarioUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuarioEjecutor))
                return Results.Unauthorized();

            var request = new ModificarPermisosUsuarioRequest(id, body.Permisos);
            var response = casoDeUso.Ejecutar(request, idUsuarioEjecutor);

            return Results.Ok(response);
        });
    }
}

public record ModificarMisDatosBody(string? NuevoNombre, string? PasswordActual, string? NuevaPassword);

public record ModificarPermisosUsuarioBody(IEnumerable<SGR.Dominio.Usuarios.Permiso> Permisos);
