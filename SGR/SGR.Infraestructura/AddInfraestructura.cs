using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.Autorizaciones;
using SGR.Infraestructura.Autorizacion;
using SGR.Infraestructura.Configuracion;
using SGR.Infraestructura.Repositorios;
using SGR.Infraestructura.Servicios;
// Agregá los usings de tu DbContext, Repositorios y Servicios

namespace SGR.Infraestructura;

public static class Extensiones
{
    public static IServiceCollection AddInfraestructura(this IServiceCollection servicios,IConfiguration configuration)
    {
        // 1. Base de datos SQLite
        var connectionString = configuration.GetConnectionString("SgrDb");
        servicios.AddDbContext<SgrContext>(options =>
            options.UseSqlite(connectionString));

        // 2. Unidad de Trabajo y Repositorios
        servicios.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
        servicios.AddScoped<IReclamoRepository, ReclamoRepository>();
        servicios.AddScoped<IActuacionRepository, ActuacionRepository>();
        servicios.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // 3. Servicios técnicos
        servicios.AddScoped<IHashService, HashService>();
        servicios.AddScoped<ITokenService, TokenService>();
        servicios.AddScoped<IAutorizacionService,AutorizacionService>();
        return servicios;
    }
}