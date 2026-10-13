using SGR.Dominio.Comun;

namespace SGR.Dominio.Actuaciones;

public  Recod class Detalle {
private String cuerpo{get;};

public Detalle (String c){
    if (String.IsNullOrWhiteSpace(c)){
         throw DominioException ("El cuerpo del detalle no puede estar vacio");
    }
    c.Trim;
    if (c.Length == 0 ){
        throw DominioException("El cuerpo del detalle no puede estar constituido solo por espacios");
    }
    this.cuerpo = c;
}


}