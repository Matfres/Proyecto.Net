using Microsoft.EntityFrameworkCore;
using SGR.Infraestructura.Configuracion;
using SGR.Aplicacion.Abstracciones;
using SGR.Infraestructura;
using SGR.Infraestructura.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Configurar la base de datos
var connectionString = builder.Configuration.GetConnectionString("SgrDb");

builder.Services.AddDbContext<SgrContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
builder.Services.AddScoped<IReclamoRepository, ReclamoRepository>();
builder.Services.AddScoped<IActuacionRepository, ActuacionRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SgrContext>();
    SgrSqlite.Inicializar(context);
}

app.Run();