namespace SGR.Aplicacion.Comun;
public class EntidadNoEncontradaException : Exception
{
    public EntidadNoEncontradaException(string mensaje)
        : base(mensaje)
    {
    }
}