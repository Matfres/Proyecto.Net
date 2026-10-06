using SGR.Dominio.Comun;

namespace SGR.Dominio.Reclamos;

public record class Asunto
{
    public string Valor { get; }

    public Asunto(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DominioException("El asunto no puede estar vacío.");

        valor = valor.Trim();

        if (valor.Length > 200)
            throw new DominioException("El asunto no puede superar los 200 caracteres.");

        Valor = valor;
    }

    public override string ToString() => Valor;
}