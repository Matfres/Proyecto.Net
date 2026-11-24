using Microsoft.Extensions.DependencyInjection;
using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.Actuaciones;
// Agregá los usings de tus carpetas de Autorización y Casos de Uso
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Reclamos;
using SGR.Aplicacion.Usuarios;
namespace SGR.Aplicacion;

public static class Extensiones
{
    public static IServiceCollection AddAplicacion(this IServiceCollection servicios)
    {
        // Servicios de Aplicacion
        servicios.AddScoped<IActualizacionEstadoReclamoService, ActualizacionEstadoReclamoService>();
        //reclamos
        servicios.AddScoped<AgregarReclamoUseCase>();
        servicios.AddScoped<CambiarEstadoReclamoUseCase>();
        servicios.AddScoped<EliminarReclamoUseCase>();
        servicios.AddScoped<ListarReclamosUseCase>();
        servicios.AddScoped<ModificarAsuntoReclamoUseCase>();
        servicios.AddScoped<ObtenerReclamoPorIdUseCase>();
        //actuaciones
        servicios.AddScoped<AgregarActuacionUseCase>();
        servicios.AddScoped<EliminarActuacionUseCase>();
        servicios.AddScoped<ListarActuacionPorReclamoUseCase>();
        servicios.AddScoped<ModificarActuacionUseCase>();
        //usuarios
        servicios.AddScoped<EliminarUsuarioUseCase>();
        servicios.AddScoped<RegistrarUsuarioUseCase>();
        servicios.AddScoped<LoginUseCase>();
        servicios.AddScoped<ModificarMisDatosUseCase>();
        servicios.AddScoped<ModificarPermisosUsuarioUseCase>();    
        servicios.AddScoped<ListarUsuariosUseCase>();    
        // Casos de Uso
        return servicios;
    }
}