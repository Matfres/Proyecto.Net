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
    
}