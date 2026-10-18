namespace SGR.Dominio.Actuaciones;
using SGR.Dominio.Comun;

public class Actuacion{
    public Guid id {get;private set;}
    public Guid ReclamoId {get;private set;}
    public  TipoActuacion tipo{get;private set;}
    public Detalle detalle {get;private set;}
    public DateTime FechaCreacion {get;private set;}
    public DateTime FechaUltimaModificacion{get;private set;}
    public Guid UsuarioUltimoCambio {get;private set;}

    private Actuacion ( Guid Id, Guid Rid,TipoActuacion Tipo, Detalle Det,DateTime Fc, DateTime Fmod, Guid UsuarioId){
        if ( Id == Guid.Empty){
            throw new DominioException( "el Id no puede ser nulo");
        }
        this.id = Id;
        if (Rid == Guid.Empty)
        {
            throw  new DominioException (" La actuacion debe tener un ID de reclamo ");
        }
        this.ReclamoId = Rid;
        if( !Enum.IsDefined(typeof(TipoActuacion), Tipo)){
            throw new DominioException("el tipo no se encuentra dentro de los tipos aceptados por una actuacion");
        }
        this.tipo = Tipo;
        this.detalle = Det ?? throw new DominioException("el detalle no puede ser nulo");
        if ( Fc == DateTime.MinValue){
            throw new DominioException("la fecha de creacion tiene que ser inicializada");
        }
        this.FechaCreacion= Fc;
        if (Fmod < Fc){
            throw new DominioException ("la fecha de modificacion no puede ser menor a la fecha de creacion");
        }
        if (Fmod == DateTime.MinValue)
        {
            throw new DominioException("la fecha de modificacion tiene que ser inicializada");
        }
        this.FechaUltimaModificacion=Fmod;
        if (UsuarioId == Guid.Empty){
            throw new DominioException("El id del usuario que ha realizado la ultima modificacion no puede ser nulo");
        }
        this.UsuarioUltimoCambio = UsuarioId;
    }
    public Actuacion(Guid Rid,TipoActuacion Tipo, Detalle Det,DateTime Fc, Guid UsuarioId) :  this(Guid.NewGuid(), Rid,Tipo, Det,Fc,Fc,UsuarioId)
    {

    }
    public static Actuacion Reconstruir (Guid Id,Guid Rid,TipoActuacion Tipo, Detalle Det,DateTime Fc,DateTime Fmod, Guid UsuarioId){
         return new Actuacion (Id,Rid,Tipo,Det,Fc,Fmod,UsuarioId);

    }
    public void ModificarDetalle(Detalle nuevoDetalle, Guid idUsuario)
    {
        if (nuevoDetalle == null)
        {
            throw new DominioException("El detalle es obligatorio.");
        }
        detalle = nuevoDetalle;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;
    }
    public void CambiarTipo(TipoActuacion nuevoTipo, Guid idUsuario)
    {
        if (!Enum.IsDefined(typeof(TipoActuacion), nuevoTipo))
        {
            throw new DominioException("El tipo de actuacion indicado no es válido.");
        }
        tipo = nuevoTipo;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;
    }
}
