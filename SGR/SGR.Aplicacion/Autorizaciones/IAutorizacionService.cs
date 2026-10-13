namespace SGR.Aplicacion.Autorizaciones;

public interface IAutorizacionService{
    bool PoseeElPermiso(Guid idUsuario, Permiso permiso);
}
