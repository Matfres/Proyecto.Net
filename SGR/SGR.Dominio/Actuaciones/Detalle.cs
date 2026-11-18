
using SGR.Dominio.Comun;

namespace SGR.Dominio.Actuaciones;

public record class Detalle
{
    public string Cuerpo { get; private set; } = "";

    public Detalle(string cuerpo)
    {
        if (string.IsNullOrWhiteSpace(cuerpo))
            throw new DominioException("El cuerpo del detalle no puede estar vacío.");

        cuerpo = cuerpo.Trim();

        Cuerpo = cuerpo;
    }

    protected Detalle()
    {
    }

    public override string ToString() => Cuerpo;
}
