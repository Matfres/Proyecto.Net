
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SGR.WebApi.Configuracion;

public static class AutenticacionJwt
{
    public static void AgregarAutenticacionJwt(this IServiceCollection servicios, IConfiguration configuracion)
    {
        var jwtIssuer = configuracion["Jwt:Issuer"];
        var jwtAudience = configuracion["Jwt:Audience"];
        var jwtSecretKey = configuracion["Jwt:SecretKey"];

        servicios.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
    }
}
