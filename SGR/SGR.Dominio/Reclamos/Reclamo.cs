
using SGR.Dominio.Comun;

namespace SGR.Dominio.Reclamos;

public class Reclamo : Entidad
{
    public Asunto Asunto { get; private set; } = null!;
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }
    public EstadoReclamo Estado { get; private set; }

    // Constructor público: crea un reclamo nuevo.
    public Reclamo(Asunto asunto, DateTime fechaCreacion, Guid idUsuario)
    {
        if (asunto == null)
            throw new DominioException("El asunto es obligatorio.");
        
        if (fechaCreacion == DateTime.MinValue)
            throw new DominioException("La fecha de creación tiene que ser inicializada.");

        ValidarIdUsuario(idUsuario);
        Asunto = asunto;
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaCreacion;
        UsuarioUltimoCambio = idUsuario;
        Estado = EstadoReclamo.Recibido;
    }

    // Constructor sin parámetros protegido, exclusivo para Entity Framework Core.
    protected Reclamo()
    {
    }

    public void ModificarAsunto(Asunto nuevoAsunto, Guid idUsuario)
    {
        if (nuevoAsunto == null)
            throw new DominioException("El asunto es obligatorio.");

        ValidarIdUsuario(idUsuario);

        Asunto = nuevoAsunto;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;
    }

    public void CambiarEstado(EstadoReclamo nuevoEstado, Guid idUsuario)
    {
        if (!Enum.IsDefined(typeof(EstadoReclamo), nuevoEstado))
            throw new DominioException("El estado indicado no es válido.");

        ValidarIdUsuario(idUsuario);

        Estado = nuevoEstado;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;
    }

    public bool ActualizarEstado(TipoActuacion? ultimoTipo, Guid idUsuario)
    {
        EstadoReclamo? nuevoEstado;
        ValidarIdUsuario(idUsuario);

        if (ultimoTipo == null)
        {
            nuevoEstado = EstadoReclamo.Recibido;
        }
        else
        {
            nuevoEstado = ultimoTipo switch
            {
                TipoActuacion.Inspeccion => EstadoReclamo.EnInspeccion,
                TipoActuacion.OrdenDeTrabajo => EstadoReclamo.EnEjecucion,
                TipoActuacion.TrabajoRealizado => EstadoReclamo.Resuelto,
                TipoActuacion.Archivo => EstadoReclamo.Cerrado,
                TipoActuacion.Observacion => null,
                TipoActuacion.RespuestaAlVecino => null,
                _ => null
            };
        }

        if (nuevoEstado == null)
            return false;

        if (Estado == nuevoEstado)
            return false;

        Estado = nuevoEstado.Value;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;

        return true;
    }

    private static void ValidarIdUsuario(Guid idUsuario)
    {
        if (idUsuario == Guid.Empty)
            throw new DominioException("El Id del usuario no puede ser un Guid vacío.");
    }
}
