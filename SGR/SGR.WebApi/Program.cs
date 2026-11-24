using SGR.Infraestructura.Configuracion;
using SGR.Infraestructura;
using SGR.WebApi.Configuracion;
using SGR.Aplicacion;
using SGR.WebApi.Endpoints;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configurar OpenAPI.
builder.Services.AddOpenApi();

// Configurar la base de datos.
builder.Services.AddInfraestructura(builder.Configuration);

// Registrar los servicios de autorización.
builder.Services.AddAuthorization();

// Registrar servicios de aplicación.
builder.Services.AddAplicacion();

// Configurar autenticación JWT.
builder.Services.AgregarAutenticacionJwt(builder.Configuration);

// Configurar la serialización de los enumerados como texto.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Configurar manejo de excepciones y autenticación.
app.UsarManejoExcepciones();
app.UseAuthentication();
app.UseAuthorization();

// Inicializar la base de datos.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SgrContext>();
    SgrSqlite.Inicializar(context);
}

// Registrar endpoints.
app.MapReclamoEndpoints();
app.MapAuthEndpoints();
app.MapUsuarioEndpoints();
app.MapActuacionEndpoints();

// Habilitar OpenAPI y Scalar en desarrollo.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Run();
