
using SGR.Dominio.Comun;

namespace SGR.Dominio.Usuarios;

public record class CorreoElectronico
{
    public string Valor { get; private set; } = "";

    public CorreoElectronico(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DominioException("El correo electrónico no puede estar vacío.");

        valor = valor.Trim().ToLowerInvariant();

        int posicionArroba = valor.IndexOf('@');

        if (posicionArroba <= 0 || posicionArroba != valor.LastIndexOf('@') || posicionArroba == valor.Length - 1)
            throw new DominioException("El correo electrónico no tiene un formato válido.");

        string dominio = valor[(posicionArroba + 1)..];

        if (!dominio.Contains('.') || dominio.StartsWith('.') || dominio.EndsWith('.'))
            throw new DominioException("El correo electrónico no tiene un formato válido.");

        Valor = valor;
    }

    protected CorreoElectronico()
    {
    }

    public override string ToString() => Valor;
}
