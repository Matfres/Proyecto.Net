using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.Comun;
using SGR.Aplicacion.DTO;
using SGR.Aplicacion.Autorizaciones;

namespace SGR.Aplicacion.Usuarios;

public class ModificarPermisosUsuarioUseCase(
    IUsuarioRepository _usuarioRepository,
    IUnidadDeTrabajo _unidadDeTrabajo)
{
    public ModificarPermisosUsuarioResponse Ejecutar(ModificarPermisosUsuarioRequest request, Guid idUsuarioEjecutor)
    {
        // 1. Regla de control: verificar en primera instancia que sea Administrador
        var ejecutor = _usuarioRepository.ObtenerPorId(idUsuarioEjecutor);
        if (ejecutor == null || !ejecutor.EsAdministrador)
            throw new AutorizacionException("Solo un administrador puede modificar permisos.");

        var usuario = _usuarioRepository.ObtenerPorId(request.IdUsuario);
            if (usuario == null)
                throw new EntidadNoEncontradaException("El usuario especificado no existe.");

        var permisosDeseados = request.Permisos.Distinct().ToList();
        var permisosActuales = usuario.Permisos.ToList(); // .ToList() evita error al modificar la colección mientras se recorre

        // Quitar los que tenía antes y ya no están en la lista nueva
        foreach (var permisoActual in permisosActuales)
{
            if (!permisosDeseados.Contains(permisoActual))
    {
                usuario.QuitarPermiso(permisoActual);
    }
}

        // Agregar los que vinieron en la lista nueva y todavía no tenía
        foreach (var permisoNuevo in permisosDeseados)
{
            if (!permisosActuales.Contains(permisoNuevo))
    {   
                usuario.AgregarPermiso(permisoNuevo);
    }
}
        _unidadDeTrabajo.GuardarCambios();

        return new ModificarPermisosUsuarioResponse(
            usuario.Id,
            usuario.CorreoElectronico.Valor,
            usuario.Nombre,
            usuario.Permisos
        );
    }
}