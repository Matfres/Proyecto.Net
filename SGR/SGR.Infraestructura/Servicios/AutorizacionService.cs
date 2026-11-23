using SGR.Aplicacion.Autorizaciones;
using SGR.Dominio.Usuarios;
using SGR.Aplicacion.Abstracciones;
using SGR.Infraestructura.Repositorios;
namespace SGR.Infraestructura.Autorizacion;

public class AutorizacionService(IUsuarioRepository _UsuarioRepository) : IAutorizacionService
{
    public bool PoseeElPermiso(Guid idUsuario, Permiso permiso)
    {
        var usuario = _UsuarioRepository.ObtenerPorId(idUsuario);
        if (usuario == null) return false;

        return usuario.PoseePermiso(permiso);
    }
    }
