using Microsoft.EntityFrameworkCore;
using SGR.Infraestructura.Configuracion;
using SGR.Aplicacion.Abstracciones;
using SGR.Infraestructura;
using SGR.Infraestructura.Repositorios;
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Actuaciones;
using SGR.Infraestructura.Autorizacion;
using SGR.Infraestructura.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar la base de datos
var connectionString = builder.Configuration.GetConnectionString("SgrDb");

builder.Services.AddDbContext<SgrContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
builder.Services.AddScoped<IReclamoRepository, ReclamoRepository>();
builder.Services.AddScoped<IActuacionRepository, ActuacionRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
// Registrar servicios
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAutorizacionService, AutorizacionService>();
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IActualizacionEstadoReclamoService, ActualizacionEstadoReclamoService>();

// Configurar autenticación JWT
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey!))
        };
    });
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SgrContext>();
    SgrSqlite.Inicializar(context);
}

app.Run();