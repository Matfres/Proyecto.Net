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

    public Reclamo(Asunto asunto, DateTime fechaCreacion, Guid idUsuario)
    {
        if (asunto == null)
            throw new DominioException("El asunto es obligatorio.");

        Id = Guid.NewGuid();
        Asunto = asunto;
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaCreacion;
        UsuarioUltimoCambio = idUsuario;
        Estado = EstadoReclamo.Recibido;
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