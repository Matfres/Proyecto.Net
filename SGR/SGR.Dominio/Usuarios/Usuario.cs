
using SGR.Dominio.Comun;

namespace SGR.Dominio.Usuarios;

public class Usuario : Entidad
{
    private List<Permiso> _permisos = [];
    public string Nombre { get; private set; } = "";
    public CorreoElectronico CorreoElectronico { get; private set; } = null!;
    public string ContrasenaHash { get; private set; } = "";
    public bool EsAdministrador { get; private set; }
    public IReadOnlyCollection<Permiso> Permisos => _permisos.AsReadOnly();

    // Constructor público: crea un usuario nuevo.
    public Usuario(string nombre, CorreoElectronico correoElectronico, string contrasenaHash, bool esAdministrador)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DominioException("El nombre del usuario no puede estar vacío.");

        if (correoElectronico == null)
            throw new DominioException("El correo electrónico del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(contrasenaHash))
            throw new DominioException("La contraseña cifrada del usuario es obligatoria.");

        Nombre = nombre;
        CorreoElectronico = correoElectronico;
        ContrasenaHash = contrasenaHash;
        EsAdministrador = esAdministrador;
    }


    public void ModificarNombre(string nuevoNombre)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new DominioException("El nombre del usuario no puede estar vacío.");

        Nombre = nuevoNombre.Trim();
    }

    public void CambiarContrasenaHash(string nuevaContrasenaHash)
    {
        if (string.IsNullOrWhiteSpace(nuevaContrasenaHash))
            throw new DominioException("La contraseña cifrada del usuario es obligatoria.");

        ContrasenaHash = nuevaContrasenaHash;
    }


    public void AgregarPermiso(Permiso permiso)
    {
        if (!Enum.IsDefined(typeof(Permiso), permiso))
            throw new DominioException("El permiso indicado no es válido.");

        if (!_permisos.Contains(permiso))
            _permisos.Add(permiso);
    }

    public void QuitarPermiso(Permiso permiso)
    {
        if (!Enum.IsDefined(typeof(Permiso), permiso))
            throw new DominioException("El permiso indicado no es válido.");

        _permisos.Remove(permiso);
    }

    public bool PoseePermiso(Permiso permiso)
    {
        if (!Enum.IsDefined(typeof(Permiso), permiso))
            throw new DominioException("El permiso indicado no es válido.");

    if ( _permisos.Contains(permiso)){
        return true;
    }
    if (permiso == Permiso.ActuacionBaja  && _permisos.Contains (Permiso.ReclamoBaja)){
        return true;
    }
    return false;
    }



    // Constructor sin parámetros protegido, exclusivo para Entity Framework Core.
    protected Usuario()
    {
    }
}
