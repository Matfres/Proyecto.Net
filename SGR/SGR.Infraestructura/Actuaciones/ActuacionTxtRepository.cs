using System.Globalization;
using System.Text;
using SGR.Aplicacion.Actuaciones;
using SGR.Dominio.Comun;
using SGR.Dominio.Actuaciones;
using SGR.Infraestructura.Comun;
namespace SGR.Infraestructura.Actuaciones;

public class ActuacionesTxtRepository : IActuacionRepository
{
    
    private readonly string _rutaArchivo;

public ActuacionesTxtRepository (string ruta)
    {
        _rutaArchivo = ruta;
    }

  private static string Serializar(Actuacion actuacion)
    {   //para evitar confusiones entre el pipe divisor de campos y un caracter del detalle se somete al detalle a una codificacion
        var detalleCodificado = Convert.ToBase64String(Encoding.UTF8.GetBytes(actuacion.detalle.ToString()));
        //convierte cada atributo en un string y los concatena con un | en medio
        return string.Join("|",
            actuacion.id,
            actuacion.ReclamoId,
            detalleCodificado,
            actuacion.FechaCreacion.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
            actuacion.FechaUltimaModificacion.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
            actuacion.UsuarioUltimoCambio,
            actuacion.tipo);
    }

    private static Actuacion Deserializar(string linea)
    {   //recibe la linea y la divide apartir del |, cada division sera un elemento en un vector
        var campos = linea.Split('|');
        //si el vector campos no cumple con la cantidad de atributos de una actuacion tira una excepcion
        if (campos.Length != 7)
            throw new RepositorioException("El registro de la actuacion tiene un formato inválido.");
        // convierte uno a uno los elementos del vector en su atributo correspondiente
        try
        {
            var id = Guid.Parse(campos[0]);
            var Rid = Guid.Parse(campos[1]);
            var detalleTexto = Encoding.UTF8.GetString(Convert.FromBase64String(campos[2]));
            var detalle = new Detalle(detalleTexto);
            var fechaCreacion = DateTime.ParseExact(campos[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var fechaUltimaModificacion = DateTime.ParseExact(campos[4], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var usuarioUltimoCambio = Guid.Parse(campos[5]);
            var tipo = Enum.Parse<TipoActuacion>(campos[6]);
        //recontruye la actuacion con el metodo estatico de la entidad
            return Actuacion.Reconstruir(id,Rid,tipo,detalle,fechaCreacion,fechaUltimaModificacion, usuarioUltimoCambio);
        }
        catch (Exception ex) when (
            ex is FormatException ||
            ex is ArgumentException ||
            ex is OverflowException)
        {   //ataja cualquier excepcion para convertirla en una RepositoryException
            throw new RepositorioException(
                $"No se pudo reconstruir la actuacion desde el registro: {ex.Message}");
        }
    }
    public IEnumerable<Actuacion> ObtenerTodos()
    {   //se crea una lista interna para operar facilmente
        var actuaciones = new List<Actuacion>();
        //si el archivo no existe se devuelve una lista vacia
        if (!File.Exists(_rutaArchivo))
            return actuaciones;

        using var lector = new StreamReader(_rutaArchivo);

        string? linea;
        //mientras la linea leida no sea nula se opera
        while ((linea = lector.ReadLine()) != null)
        {   //se ignoran las lineas vacias, "continue" saltea la iteracion actual del while
            if (string.IsNullOrWhiteSpace(linea))
                continue;

            actuaciones.Add(Deserializar(linea));
        }
        //la coleccion lista se puede castear a  un inumerable, por eso se devuelve sin problema
        return actuaciones;
    }
    private void GuardarTodos(List<Actuacion> actuaciones)
    {
        using var escritor = new StreamWriter(_rutaArchivo, false);
        //por cada elemento dentro de la lista se serializa y se escribe en el archivo
        foreach (var actuacion in actuaciones)
            escritor.WriteLine(Serializar(actuacion));
    }
  public void Agregar(Actuacion actuacion)
    {
        var actuaciones = ObtenerTodos();

        if (actuaciones.Any(a => a.id == actuacion.id))
            throw new RepositorioException("Ya existe una actuacion con ese Id.");

        using var escritor = new StreamWriter(_rutaArchivo, true);
        escritor.WriteLine(Serializar(actuacion));
    }
        public Actuacion? ObtenerPorId(Guid id)
    {
        return ObtenerTodos().FirstOrDefault(a => a.id == id);
    }
public IEnumerable<Actuacion> ObtenerReclamoId (Guid reclamoId)
    {   //obtiene todos las actuaciones y deja solo las que coinciden con el reclamo buscado
        var actuaciones = ObtenerTodos().ToList();
        actuaciones.RemoveAll (a => a.ReclamoId != reclamoId);
        //si no hay actuaciones para ese reclamo solo devuelve un enumerable vacio
        return actuaciones;
    }
    public void Modificar(Actuacion actuacion)
    {   //obtiene todas las actuaciones en una lista y ubica la actuacion con el id buscado
        var actuaciones = ObtenerTodos().ToList();
        var indice = actuaciones.FindIndex(a => a.id == actuacion.id);
        //si no se ubica a la actuacion buscada "indice" retorna -1, en ese caso ocurre una excepcion
        if (indice == -1)
            throw new RepositorioException($"No existe la actuacion con ID {actuacion.id}.");
        //se actualiza la actuacion
        actuaciones[indice] = actuacion;
        GuardarTodos(actuaciones);
    }
    public void Eliminar(Guid id)
    {      //obtiene todas las actuaciones en una lista y guarda la cantidad actual 
        var actuaciones = ObtenerTodos().ToList();
        var cantidadAnterior = actuaciones.Count;
        //elimina todas las actuaciones que tengan el id buscado
        actuaciones.RemoveAll(a => a.id == id);
        //si la cantidad de actuaciones se mantuvo igual se tira excepcion
        if (actuaciones.Count == cantidadAnterior)
            throw new RepositorioException($"No existe la actuacion con ID {id}.");

        GuardarTodos(actuaciones);
    }

}//fin
