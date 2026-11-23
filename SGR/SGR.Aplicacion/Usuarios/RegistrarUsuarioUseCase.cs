using SGR.Aplicacion.Abstracciones;
using SGR.Aplicacion.DTO;
using SGR.Dominio.Comun;
using SGR.Dominio.Usuarios;

namespace SGR.Aplicacion.Usuarios;

public class RegistrarUsuarioUseCase(
    IUsuarioRepository _usuarioRepository,
    IHashService _hashService,
    IUnidadDeTrabajo _uow)
{
    public RegistrarUsuarioResponse Ejecutar(RegistrarUsuarioRequest request)
    {
        // 1. Validar formato del correo mediante Value Object (lanza DominioException si es inválido)
        var correo = new CorreoElectronico(request.Correo);

        // 2. Verificar que el correo no esté ya registrado
        var usuarioExistente = _usuarioRepository.ObtenerPorEmail(correo);
        if (usuarioExistente != null)
        {
            throw new DominioException("Ya existe un usuario registrado con ese correo electrónico.");
        }

        // 3. Validar regla de longitud mínima de contraseña (al menos 8 caracteres)
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            throw new DominioException("La contraseña debe tener al menos 8 caracteres.");
        }

        // 4. Hashear la contraseña y crear la entidad de dominio
        // (Por defecto en el constructor de Usuario: EsAdministrador = false y Permisos vacío)
        var passwordHash = _hashService.Hashear(request.Password);
        var nuevoUsuario = new Usuario(request.Nombre,correo, passwordHash,false);

        // 5. Agregar al repositorio y persistir atómicamente con la Unidad de Trabajo
        _usuarioRepository.Agregar(nuevoUsuario);
        _uow.GuardarCambios();

        return new RegistrarUsuarioResponse(
            nuevoUsuario.Id,
            nuevoUsuario.CorreoElectronico.Valor,
            nuevoUsuario.Nombre,
            nuevoUsuario.EsAdministrador
        );
    }
}