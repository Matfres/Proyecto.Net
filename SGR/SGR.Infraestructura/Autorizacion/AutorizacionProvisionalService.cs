using SGR.Aplicacion.Autorizaciones;
using SGR.Dominio.Usuarios;
namespace SGR.Infraestructura.Autorizacion;

public class AutorizacionProvisionalService : IAutorizacionService
{
    public bool PoseeElPermiso(Guid idUsuario, Permiso permiso)
    {
        return true;
    }
}