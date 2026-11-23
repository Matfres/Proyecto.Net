
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Comun;
using SGR.Dominio.Comun;

namespace SGR.WebApi.Configuracion;

public static class ManejoExcepciones
{
    public static void UsarManejoExcepciones(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
        {
            var excepcion = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            var (codigo, titulo, detalle) = excepcion switch
            {
                DominioException ex => (400, "Error de dominio", ex.Message),
                AutenticacionException => (401, "No autenticado", "Credenciales inválidas."),
                AutorizacionException ex => (403, "Acceso denegado", ex.Message),
                EntidadNoEncontradaException ex => (404, "Recurso no encontrado", ex.Message),
                CorreoElectronicoDuplicadoException ex => (409, "Conflicto", ex.Message),
                _ => (500, "Error interno del servidor", "Ocurrió un error inesperado.")
            };

            context.Response.StatusCode = codigo;

            var problem = new ProblemDetails
            {
                Status = codigo,
                Title = titulo,
                Detail = detalle,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problem);
        }));
    }
}
