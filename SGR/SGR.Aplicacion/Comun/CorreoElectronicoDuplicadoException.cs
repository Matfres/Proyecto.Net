namespace SGR.Aplicacion.Comun;

public class CorreoElectronicoDuplicadoException : Exception
{
    public CorreoElectronicoDuplicadoException(string mensaje) : base(mensaje)
    {
    }
}