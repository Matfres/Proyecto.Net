using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.Comun;
using SGR.Aplicacion.DTO;
using SGR.Aplicacion.Autorizaciones;
using SGR.Dominio.Comun;

namespace SGR.Aplicacion.Usuarios;

public class EliminarUsuarioUseCase(
    IUsuarioRepository _usuarioRepository,
    IUnidadDeTrabajo _unidadDeTrabajo)
{
    public EliminarUsuarioResponse Ejecutar(EliminarUsuarioRequest request, Guid idUsuarioEjecutor)
    {
        // 1. Regla de control: verificar en primera instancia que quien ejecuta sea Administrador
        var ejecutor = _usuarioRepository.ObtenerPorId(idUsuarioEjecutor);
        if (ejecutor == null || !ejecutor.EsAdministrador)
        {
            throw new AutorizacionException("Acceso denegado. Solo un administrador puede eliminar usuarios.");
        }

        // 2. Regla de negocio: un administrador no puede eliminarse a sí mismo
        if (request.IdUsuario == idUsuarioEjecutor)
        {
            throw new DominioException("Un administrador no puede eliminarse a sí mismo.");
        }

        // 3. Buscar el usuario objetivo y verificar que exista
        var usuarioAEliminar = _usuarioRepository.ObtenerPorId(request.IdUsuario);
        if (usuarioAEliminar == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el usuario con ID {request.IdUsuario}.");
        }

        // 4. Eliminar y guardar en una única transacción atómica
        _usuarioRepository.Eliminar(usuarioAEliminar.Id);
        _unidadDeTrabajo.GuardarCambios();

        return new EliminarUsuarioResponse(usuarioAEliminar.Id);
    }
}