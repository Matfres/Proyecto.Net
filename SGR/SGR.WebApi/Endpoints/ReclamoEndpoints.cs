
using System.Security.Claims;
using SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.DTO;
using SGR.Aplicacion.Reclamos;
using SGR.Dominio.Comun;

namespace SGR.WebApi.Endpoints;

public static class ReclamoEndpoints
{
    public static void MapReclamoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var grupo = endpoints.MapGroup("/reclamos")
            .RequireAuthorization()
            .WithTags("Reclamos");

        // Crear un reclamo.
        grupo.MapPost("/", (AgregarReclamoBody body, ClaimsPrincipal usuario, AgregarReclamoUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            var request = new AgregarReclamoRequest(body.Asunto, idUsuario);
            var response = casoDeUso.Ejecutar(request);

            return Results.Created($"/reclamos/{response.Id}", response);
        });

        // Listar reclamos, con filtro opcional por estado.
        grupo.MapGet("/", (EstadoReclamo? estado, ListarReclamosUseCase casoDeUso) =>
        {
            var request = new ListarReclamosRequest(estado);
            var response = casoDeUso.Ejecutar(request);

            return Results.Ok(response);
        });

        // Obtener un reclamo por ID, incluyendo sus actuaciones.
        grupo.MapGet("/{id:guid}", (Guid id, ObtenerReclamoPorIdUseCase casoDeUso) =>
        {
            var request = new ObtenerReclamoPorIdRequest(id);
            var response = casoDeUso.Ejecutar(request);

            return Results.Ok(response);
        });

        // Modificar el asunto de un reclamo.
        grupo.MapPut("/{id:guid}/asunto", (Guid id, ModificarAsuntoReclamoBody body, ClaimsPrincipal usuario, ModificarAsuntoReclamoUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            var request = new ModificarAsuntoReclamoRequest(id, body.NuevoAsunto, idUsuario);
            var response = casoDeUso.Ejecutar(request);

            return Results.Ok(response);
        });

        // Cambiar el estado de un reclamo.
        grupo.MapPut("/{id:guid}/estado", (Guid id, CambiarEstadoReclamoBody body, ClaimsPrincipal usuario, CambiarEstadoReclamoUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            var request = new CambiarEstadoReclamoRequest(id, body.NuevoEstado, idUsuario);
            var response = casoDeUso.Ejecutar(request);

            return Results.Ok(response);
        });

        // Eliminar un reclamo y sus actuaciones asociadas.
        grupo.MapDelete("/{id:guid}", (Guid id, ClaimsPrincipal usuario, EliminarReclamoUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            var request = new EliminarReclamoRequest(id, idUsuario);
            var response = casoDeUso.Ejecutar(request);

            return Results.Ok(response);
        });

        // Listar las actuaciones asociadas a un reclamo.
        grupo.MapGet("/{id:guid}/actuaciones", (Guid id, ListarActuacionPorReclamoUseCase casoDeUso) =>
        {
            var request = new ListarActuacionPorReclamoRequest(id);
            var response = casoDeUso.Ejecutar(request);

            return Results.Ok(response);
        });
    }
}

public record AgregarReclamoBody(string Asunto);

public record ModificarAsuntoReclamoBody(string NuevoAsunto);

public record CambiarEstadoReclamoBody(EstadoReclamo NuevoEstado);
