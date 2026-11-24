
using System.Security.Claims;
using SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.DTO;
using SGR.Dominio.Comun;

namespace SGR.WebApi.Endpoints;

public static class ActuacionEndpoints
{
    public static void MapActuacionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Endpoints para agregar actuaciones a un reclamo.
        var grupoReclamos = endpoints.MapGroup("/reclamos/{id:guid}/actuaciones")
            .RequireAuthorization()
            .WithTags("Actuaciones");

        // Agregar una actuación a un reclamo.
        grupoReclamos.MapPost("/", (Guid id, AgregarActuacionBody body, ClaimsPrincipal usuario, AgregarActuacionUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            var request = new AgregarActuacionRequest(body.Detalle, idUsuario, body.Tipo, id);
            var response = casoDeUso.Ejecutar(request);

            return Results.Created($"/actuaciones/{response.Id}", response);
        });

        // Endpoints para modificar actuaciones.
        var grupoActuaciones = endpoints.MapGroup("/actuaciones")
            .RequireAuthorization()
            .WithTags("Actuaciones");

        // Modificar una actuación.
        grupoActuaciones.MapPut("/{id:guid}", (Guid id, ModificarActuacionBody body, ClaimsPrincipal usuario, ModificarActuacionUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            var request = new ModificarActuacionRequest(id, idUsuario, body.Tipo, body.Detalle);
            var response = casoDeUso.Ejecutar(request);

            return Results.Ok(response);
        });

        // Eliminar una actuación.
        grupoActuaciones.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal usuario, EliminarActuacionUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            var request = new EliminarActuacionRequest(id, idUsuario);
            var response = casoDeUso.Ejecutar(request);

            return Results.Ok(response);
        });
    }
}

public record AgregarActuacionBody(string Detalle, TipoActuacion Tipo);

public record ModificarActuacionBody(TipoActuacion Tipo, string Detalle);
