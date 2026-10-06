namespace SGR.Dominio.Comun;

public class DominioException : Exception
{
    public DominioException(string mensaje)
        : base(mensaje)
    {
    }
}