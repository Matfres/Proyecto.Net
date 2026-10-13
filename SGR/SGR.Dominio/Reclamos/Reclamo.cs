using SGR.Dominio.Comun;

namespace SGR.Dominio.Reclamos;

public class Reclamo
{
    public Guid Id { get; private set; }
    public Asunto Asunto { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }
    public EstadoReclamo Estado { get; private set; }

      // Constructor público: crea un reclamo nuevo.
    public Reclamo(Asunto asunto, DateTime fechaCreacion, Guid idUsuario): this(Guid.NewGuid(), asunto, fechaCreacion, fechaCreacion, idUsuario, EstadoReclamo.Recibido)
    {
    }

    // Constructor privado: centraliza las validaciones y asignaciones.
    private Reclamo(Guid id, Asunto asunto, DateTime fechaCreacion, DateTime fechaUltimaModificacion, Guid usuarioUltimoCambio, EstadoReclamo estado)
    {
        if (id == Guid.Empty)
            throw new DominioException("El Id del reclamo no puede ser un Guid vacío.");

        if (asunto == null)
            throw new DominioException("El asunto es obligatorio.");

        if (fechaUltimaModificacion < fechaCreacion)
            throw new DominioException("La fecha de última modificación no puede ser anterior a la fecha de creación.");

        if (!Enum.IsDefined(typeof(EstadoReclamo), estado))
            throw new DominioException("El estado indicado no es válido.");

        Id = id;
        Asunto = asunto;
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaUltimaModificacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
        Estado = estado;
    }

    // Factory Method: reconstruye un reclamo existente.
    public static Reclamo Reconstruir(Guid id, Asunto asunto, DateTime fechaCreacion, DateTime fechaUltimaModificacion, Guid usuarioUltimoCambio, EstadoReclamo estado)
    {
        return new Reclamo(id, asunto, fechaCreacion, fechaUltimaModificacion, usuarioUltimoCambio, estado);
    }

    public void ModificarAsunto(Asunto nuevoAsunto, Guid idUsuario)
    {
        if (nuevoAsunto == null)
        {
            throw new DominioException("El asunto es obligatorio.");
        }
        Asunto = nuevoAsunto;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;
    }

    public void CambiarEstado(EstadoReclamo nuevoEstado, Guid idUsuario)
    {
        if (!Enum.IsDefined(typeof(EstadoReclamo), nuevoEstado))
        {
            throw new DominioException("El estado indicado no es válido.");
        }
        Estado = nuevoEstado;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;
    }

    public bool ActualizarEstado(TipoActuacion? ultimoTipo, Guid idUsuario)
    {
        EstadoReclamo? nuevoEstado;

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
        {   
            return false;
        }
        
        if (Estado == nuevoEstado) // si estado previo es el mismo que actual que llega no impacta 
        {
            return false;
        }

        Estado = nuevoEstado.Value;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;

        return true;
    }
    
}