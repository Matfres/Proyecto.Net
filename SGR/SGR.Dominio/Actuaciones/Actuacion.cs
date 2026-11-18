
using SGR.Dominio.Comun;

namespace SGR.Dominio.Actuaciones;

public class Actuacion
{
    public Guid id { get; private set; }
    public Guid ReclamoId { get; private set; }
    public TipoActuacion tipo { get; private set; }
    public Detalle detalle { get; private set; } = null!;
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }

    // Constructor público: crea una actuación nueva.
    public Actuacion(Guid reclamoId, TipoActuacion unTipo, Detalle unDetalle, DateTime fechaCreacion, Guid usuarioUltimoCambio)
    {
        if (reclamoId == Guid.Empty)
            throw new DominioException("La actuación debe tener un ID de reclamo.");

        if (!Enum.IsDefined(typeof(TipoActuacion), tipo))
            throw new DominioException("El tipo no se encuentra dentro de los tipos aceptados por una actuación.");

        if (detalle == null)
            throw new DominioException("El detalle no puede ser nulo.");

        if (fechaCreacion == DateTime.MinValue)
            throw new DominioException("La fecha de creación tiene que ser inicializada.");

        ValidarIdUsuario(usuarioUltimoCambio);

        id = Guid.NewGuid();
        ReclamoId = reclamoId;
        tipo = unTipo;
        detalle = unDetalle;
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaCreacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
    }

    // Constructor sin parámetros protegido, exclusivo para Entity Framework Core.
    protected Actuacion() { }

    public void ModificarDetalle(Detalle nuevoDetalle, Guid idUsuario)
    {
        if (nuevoDetalle == null)
            throw new DominioException("El detalle es obligatorio.");

        ValidarIdUsuario(idUsuario);

        detalle = nuevoDetalle;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;
    }

    public void CambiarTipo(TipoActuacion nuevoTipo, Guid idUsuario)
    {
        if (!Enum.IsDefined(typeof(TipoActuacion), nuevoTipo))
            throw new DominioException("El tipo de actuación indicado no es válido.");

        ValidarIdUsuario(idUsuario);

        tipo = nuevoTipo;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;
    }

    private static void ValidarIdUsuario(Guid idUsuario)
    {
        if (idUsuario == Guid.Empty)
            throw new DominioException("El ID del usuario no puede estar vacío.");
    }
}
