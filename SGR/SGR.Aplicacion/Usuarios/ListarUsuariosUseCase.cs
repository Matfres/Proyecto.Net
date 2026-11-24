using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.DTO;

namespace SGR.Aplicacion.Usuarios;

public class ListarUsuariosUseCase(IUsuarioRepository _usuarioRepository)
{
    public ListarUsuariosResponse Ejecutar(Guid idUsuarioEjecutor)
    {
        // 1. Regla de control: verificar en primera instancia que sea Administrador
        var ejecutor = _usuarioRepository.ObtenerPorId(idUsuarioEjecutor);
        if (ejecutor is null || !ejecutor.EsAdministrador)
        {
            throw new AutorizacionException("Acceso denegado. Solo un administrador puede listar los usuarios.");
        }

        // 2. Obtener todos los usuarios desde el repositorio
        var usuarios = _usuarioRepository.ObtenerTodos();

        // 3. Proyectar al DTO omitiendo por completo el hash de la contraseña
        var usuariosDto = usuarios.Select(u => new UsuarioDTO(
            u.Id,
            u.CorreoElectronico.Valor,
            u.Nombre,
            u.EsAdministrador,
            u.Permisos
        )).ToList();

        return new ListarUsuariosResponse(usuariosDto);
    }
}