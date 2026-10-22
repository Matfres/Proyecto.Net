using SGR.Aplicacion.Autorizaciones;

namespace SGR.Infraestructura.Autorizacion;

public class AutorizacionProvisionalService : IAutorizacionService
{
    public bool PoseeElPermiso(Guid idUsuario, Permiso permiso)
    {
        return false;
    }
}