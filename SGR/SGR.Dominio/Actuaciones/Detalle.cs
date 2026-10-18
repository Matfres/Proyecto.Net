using SGR.Dominio.Comun;

namespace SGR.Dominio.Actuaciones;

public  record class Detalle {
private String cuerpo{get;}

public Detalle (String c){
    if (String.IsNullOrWhiteSpace(c)){
         throw new DominioException ("El cuerpo del detalle no puede estar vacio");
    }
    c.Trim();
    if (c.Length == 0 ){
        throw new DominioException("El cuerpo del detalle no puede estar constituido solo por espacios");
    }
    this.cuerpo = c;
}

    public override string ToString() => cuerpo;
}