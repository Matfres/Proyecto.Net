
namespace SGR.Aplicacion.Abstracciones;

public interface IUnidadDeTrabajo
{
    /// <summary>
    /// Confirma y guarda en el medio de persistencia todos los cambios 
    /// acumulados en memoria por los repositorios durante la transacción actual.
    /// </summary>
    void GuardarCambios();
}