
using System.Security.Claims;
using SGR.Aplicacion.DTO;
using SGR.Aplicacion.Reclamos;

namespace SGR.WebApi.Endpoints;

public static class ReclamoEndpoints
{
    public static void MapReclamoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var grupo = endpoints.MapGroup("/reclamos").RequireAuthorization();

        grupo.MapPost("/", (AgregarReclamoBody body, ClaimsPrincipal usuario, AgregarReclamoUseCase casoDeUso) =>
        {
            var idUsuarioClaim = usuario.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(idUsuarioClaim, out var idUsuario))
                return Results.Unauthorized();

            var request = new AgregarReclamoRequest(body.Asunto, idUsuario);
            var response = casoDeUso.Ejecutar(request);

            return Results.Created($"/reclamos/{response.Id}", response);
        });
    }
}

public record AgregarReclamoBody(string Asunto);
