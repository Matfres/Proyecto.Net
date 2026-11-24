using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.Comun;
using SGR.Aplicacion.DTO;
using SGR.Dominio.Comun;
using SGR.Dominio.Usuarios;

namespace SGR.Aplicacion.Usuarios;

public class LoginUseCase(
    IUsuarioRepository _usuarioRepository,
    IHashService _hashService,
    ITokenService _tokenService)
{
    public LoginResponse Ejecutar(LoginRequest request)
    {
        // 1. Validar el formato del correo mediante el Value Object.
        // Si el formato es inválido, atrapamos DominioException y lanzamos AutenticacionException
        // para no dar pistas sobre cuál dato falló.
        CorreoElectronico correo;
        try
        {
            correo = new CorreoElectronico(request.Correo);
        }
        catch (DominioException)
        {
            throw new AutenticacionException("Credenciales inválidas");
        }

        // 2. Buscar el usuario por su correo electrónico
        var usuario = _usuarioRepository.ObtenerPorEmail(correo);

        // 3. Verificar en un solo paso que exista y que la contraseña coincida
        if (usuario is null ||
            string.IsNullOrWhiteSpace(request.Password) ||
            _hashService.Hashear(request.Password) != usuario.ContrasenaHash)
        {
            throw new AutenticacionException("Credenciales inválidas");
        }

        // 4. Generar y retornar el JWT con la identidad del usuario (UserId)
        var token = _tokenService.GenerarToken(usuario.Id,usuario.Nombre);

        return new LoginResponse(token);
    }
}