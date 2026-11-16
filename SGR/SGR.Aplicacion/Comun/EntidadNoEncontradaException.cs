namespace SGR.Aplicacion;
public class EntidadNoEncontradaException : Exception
{
    public EntidadNoEncontradaException(string mensaje)
        : base(mensaje)
    {
    }
}