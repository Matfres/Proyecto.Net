using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.Comun;
using SGR.Aplicacion.DTO;
using SGR.Dominio.Comun;
using SGR.Dominio.Usuarios;

namespace SGR.Aplicacion.Usuarios;

public class ModificarMisDatosUseCase(
    IUsuarioRepository _usuarioRepository,
    IHashService _hashService,
    IUnidadDeTrabajo _unidadDeTrabajo)
{
    public ModificarMisDatosResponse Ejecutar(ModificarMisDatosRequest request, Guid idUsuario)
    {
        // 1. Verificar que haya enviado al menos un campo para actualizar
        if (request.NuevoNombre == null && request.NuevaPassword == null)
        {
            throw new DominioException("Debe indicar al menos un dato para modificar (nombre o contraseña).");
        }

        // 2. Buscar al usuario usando SIEMPRE el ID extraído del token JWT
        var usuario = _usuarioRepository.ObtenerPorId(idUsuario);
        if (usuario == null)
        {
            throw new EntidadNoEncontradaException("No se encontró el usuario autenticado.");
        }

        // 3. Si envió NuevoNombre, delegar la validación y mutación a la entidad de dominio
        if (request.NuevoNombre is not null)
        {
            usuario.ModificarNombre(request.NuevoNombre);
        }

        // 4. Si envió NuevaPassword, exigir y verificar primero la contraseña actual
        if (request.NuevaPassword is not null)
        {
            if (string.IsNullOrWhiteSpace(request.PasswordActual))
            {
                throw new DominioException("Debe ingresar su contraseña actual para realizar el cambio.");
            }

            if (_hashService.Hashear(request.PasswordActual) != usuario.ContrasenaHash)
            {
                throw new AutenticacionException("La contraseña actual es incorrecta.");
            }

            if (string.IsNullOrWhiteSpace(request.NuevaPassword) || request.NuevaPassword.Length < 8)
            {
                throw new DominioException("La nueva contraseña debe tener al menos 8 caracteres.");
            }

            var nuevoHash = _hashService.Hashear(request.NuevaPassword);
            usuario.CambiarContrasenaHash(nuevoHash);
        }

        // 5. Persistir los cambios atómicamente
        _unidadDeTrabajo.GuardarCambios();

        return new ModificarMisDatosResponse(
            usuario.Id,
            usuario.CorreoElectronico.Valor,
            usuario.Nombre
        );
    }
}