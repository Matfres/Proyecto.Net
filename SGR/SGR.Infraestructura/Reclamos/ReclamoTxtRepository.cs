
using System.Globalization;
using System.Text;
using SGR.Aplicacion.Reclamos;
using SGR.Dominio.Comun;
using SGR.Dominio.Reclamos;

namespace SGR.Infraestructura.Reclamos;

public class ReclamoTxtRepository : IReclamoRepository
{
    private readonly string _rutaArchivo;

    public ReclamoTxtRepository(string rutaArchivo)
    {
        _rutaArchivo = rutaArchivo;
    }

    public void Agregar(Reclamo reclamo)
    {
        var reclamos = ObtenerTodos();

        if (reclamos.Any(r => r.Id == reclamo.Id))
            throw new RepositorioException("Ya existe un reclamo con ese Id.");

        using var escritor = new StreamWriter(_rutaArchivo, true);
        escritor.WriteLine(Serializar(reclamo));
    }

    public Reclamo? ObtenerPorId(Guid id)
    {
        return ObtenerTodos().FirstOrDefault(r => r.Id == id);
    }

    public void Modificar(Reclamo reclamo)
    {
        var reclamos = ObtenerTodos().ToList();
        var indice = reclamos.FindIndex(r => r.Id == reclamo.Id);

        if (indice == -1)
            throw new RepositorioException($"No existe el reclamo {reclamo.Id}.");

        reclamos[indice] = reclamo;
        GuardarTodos(reclamos);
    }

    public void Eliminar(Guid id)
    {
        var reclamos = ObtenerTodos().ToList();
        var cantidadAnterior = reclamos.Count;

        reclamos.RemoveAll(r => r.Id == id);

        if (reclamos.Count == cantidadAnterior)
            throw new RepositorioException($"No existe el reclamo {id}.");

        GuardarTodos(reclamos);
    }

    public IEnumerable<Reclamo> ObtenerTodos()
    {
        var reclamos = new List<Reclamo>();

        if (!File.Exists(_rutaArchivo))
            return reclamos;

        using var lector = new StreamReader(_rutaArchivo);

        string? linea;

        while ((linea = lector.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(linea))
                continue;

            reclamos.Add(Deserializar(linea));
        }

        return reclamos;
    }

    private void GuardarTodos(List<Reclamo> reclamos)
    {
        using var escritor = new StreamWriter(_rutaArchivo, false);

        foreach (var reclamo in reclamos)
            escritor.WriteLine(Serializar(reclamo));
    }

    private static string Serializar(Reclamo reclamo)
    {
        var asuntoCodificado = Convert.ToBase64String(Encoding.UTF8.GetBytes(reclamo.Asunto.Valor));

        return string.Join("|",
            reclamo.Id,
            asuntoCodificado,
            reclamo.FechaCreacion.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
            reclamo.FechaUltimaModificacion.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
            reclamo.UsuarioUltimoCambio,
            reclamo.Estado);
    }

    private static Reclamo Deserializar(string linea)
    {
        var campos = linea.Split('|');

        if (campos.Length != 6)
            throw new RepositorioException("El registro de reclamo tiene un formato inválido.");

        try
        {
            var id = Guid.Parse(campos[0]);
            var asuntoTexto = Encoding.UTF8.GetString(Convert.FromBase64String(campos[1]));
            var asunto = new Asunto(asuntoTexto);
            var fechaCreacion = DateTime.ParseExact(campos[2], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var fechaUltimaModificacion = DateTime.ParseExact(campos[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var usuarioUltimoCambio = Guid.Parse(campos[4]);
            var estado = Enum.Parse<EstadoReclamo>(campos[5]);

            return Reclamo.Reconstruir(id, asunto, fechaCreacion, fechaUltimaModificacion, usuarioUltimoCambio, estado);
        }
        catch (Exception ex) when (
            ex is FormatException ||
            ex is ArgumentException ||
            ex is OverflowException)
        {
            throw new RepositorioException(
                $"No se pudo reconstruir el reclamo desde el registro: {ex.Message}");
        }
    }
}
