namespace SGR.Aplicacion;

public class Actuacion{
    public Guid id {get;private set};
    public Guid ReclamoId {get;private set};
    public  TipoActuacion tipo{get;private set};
    private Detalle detalle {get;private set};
    private DateTime FechaCreacion {get;private set};
    private DateTime FechaUltimaModificacion{get;private set};
    private Guid UsuarioUltimoCambio {get;private set};

    private Actuacion ( Guid Id, Guid Rid,TipoActuacion Tipo, Detalle Det,DateTime Fc, DateTime Fmod, Guid UsuarioId){
        if ( Id == Guid.isEmpty){
            throw new DominioException( "el Id no puede ser nulo");
        }
        this.id = Id
        this.ReclamoId = Rid ?? throw  new DominioException (" La actuacion debe tener un ID de reclamo ");
        if( !(Enum.IsDefined(typeof(TipoActuacion), Tipo))){
            throw new DominioException("el tipo no se encuentra dentro de los tipos aceptados por una actuacion");
        }
        this.tipo = Tipo?? throw new DominioException("el tipo no puede ser nulo");
        this.detalle = Det ?? throw ("el detalle no puede ser nulo");
        if ( Fc == DateTime.MinValue){
            throw new DominioException("la fecha de creacion tiene que ser inicializada");
        }
        this.FechaCreacion= Fc;
        if (Fmod < Fc){
            throw DominioException ("la fecha de modificacion no puede ser menor a la fecha de creacion")
        }
        this.FechaUltimaModificacion=Fmod ?? throw new DominioException("la fecha de modificacion no puede ser nulo") ;
        if (UsuarioId == Guid.isEmpty){
            throw new DominioException("El id del usuario que ha realizado la ultima modificacion no puede ser nulo");
        }
        this.UsuarioUltimoCambio = UsuarioId ?? throw new DominioException ("la modificacion debe corresponder a algun usuario");
    }
    public Actuacion(Guid Rid,TipoActuacion Tipo, Detalle Det,DateTime Fc, Guid UsuarioId){
        Fmod=Fc;
        return this(Guid.NewGuid, Rid,Tipo, Det,Fc,Fmod,UsuarioId);
    }
    public static Actuacion Reconstruir (Guid Id,Guid Rid,TipoActuacion Tipo, Detalle Det,DateTime Fc,DateTime Fmod, Guis UsuarioId){
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
            throw new DominioException("El tipon de actuacion indicado no es válido.");
        }
        tipo = nuevoTipo;
        FechaUltimaModificacion = DateTime.Now;
        UsuarioUltimoCambio = idUsuario;
    }
}
