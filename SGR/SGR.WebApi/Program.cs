using Microsoft.EntityFrameworkCore;
using SGR.Infraestructura.Configuracion;
using SGR.Aplicacion.Abstracciones;
using SGR.Infraestructura;
using SGR.Infraestructura.Repositorios;
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Actuaciones;
using SGR.Infraestructura.Autorizacion;
using SGR.Infraestructura.Servicios;
using SGR.WebApi.Configuracion;
using SGR.Aplicacion;
using SGR.Infraestructura;

var builder = WebApplication.CreateBuilder(args);

// Configurar la base de datos

builder.Services.AddInfraestructura(builder.Configuration);
// Registrar servicios
builder.Services.AddAplicacion();
// Configurar autenticación JWT
builder.Services.AgregarAutenticacionJwt(builder.Configuration);
var app = builder.Build();

app.UsarManejoExcepciones();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SgrContext>();
    SgrSqlite.Inicializar(context);
}

app.Run();
