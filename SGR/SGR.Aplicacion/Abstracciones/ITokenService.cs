namespace SGR.Aplicacion.Abstracciones;

public interface ITokenService
{
    string GenerarToken(Guid idUsuario, string nombre);
}