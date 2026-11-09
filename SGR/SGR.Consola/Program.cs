
using SGR.Aplicacion.Actuaciones;
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Reclamos;
using SGR.Dominio.Comun;
using SGR.Infraestructura.Actuaciones;
using SGR.Infraestructura.Autorizacion;
using SGR.Infraestructura.Reclamos;

// ============================================================
// 1. PREPARAMOS LOS ARCHIVOS Y LOS REPOSITORIOS
// ============================================================

var rutaDatos = Path.Combine(Environment.CurrentDirectory, "Datos");

Directory.CreateDirectory(rutaDatos);

var rutaReclamos = Path.Combine(rutaDatos, "reclamos.txt");
var rutaActuaciones = Path.Combine(rutaDatos, "actuaciones.txt");

IReclamoRepository reclamoRepository = new ReclamoTxtRepository(rutaReclamos);

IActuacionRepository actuacionRepository = new ActuacionesTxtRepository(rutaActuaciones);

// ============================================================
// 2. CREAMOS LOS SERVICIOS DE INFRAESTRUCTURA
// ============================================================

IAutorizacionService autorizacionService =new AutorizacionProvisionalService();

IActualizacionEstadoReclamoService actualizacionEstadoReclamoService =new ActualizacionEstadoReclamoService(reclamoRepository, actuacionRepository);

// ============================================================
// 3. CREAMOS LOS CASOS DE USO
// ============================================================

// Reclamos

var agregarReclamo =new AgregarReclamoUseCase(reclamoRepository, autorizacionService);
var obtenerReclamo =new ObtenerReclamoPorIdUseCase(reclamoRepository);
var listarReclamos =new ListarReclamosUseCase(reclamoRepository);
var modificarAsunto =new ModificarAsuntoReclamoUseCase(reclamoRepository, autorizacionService);
var cambiarEstado =new CambiarEstadoReclamoUseCase(reclamoRepository, autorizacionService);
var eliminarReclamo =new EliminarReclamoUseCase(reclamoRepository, actuacionRepository, autorizacionService);

// Actuaciones
var agregarActuacion =new AgregarActuacionUseCase(actuacionRepository, autorizacionService, reclamoRepository, actualizacionEstadoReclamoService);
var listarActuaciones =new ListarActuacionPorReclamoUseCase(actuacionRepository);
var modificarActuacion =new ModificarActuacionUseCase(actuacionRepository, autorizacionService, actualizacionEstadoReclamoService);
var eliminarActuacion =new EliminarActuacionUseCase(actuacionRepository, reclamoRepository, actualizacionEstadoReclamoService, autorizacionService);

// ============================================================
// 4. PREPARAMOS LAS FUNCIONES AUXILIARES
// ============================================================

var idUsuario = Guid.NewGuid();

void MostrarEstado(Guid idReclamo, string mensaje)
{
    var response = obtenerReclamo.Ejecutar(new ObtenerReclamoPorIdRequest(idReclamo));
    Console.WriteLine($"{mensaje}: {response.Reclamo.Estado}");
}

void MostrarError(Exception ex)
{
    switch (ex)
    {
        case DominioException:
            Console.WriteLine($"Error de dominio: {ex.Message}");
            break;

        case AutorizacionException:
            Console.WriteLine($"Error de autorización: {ex.Message}");
            break;

        default:
            Console.WriteLine($"Error general: {ex.Message}");
            break;
    }
}

// ============================================================
// 5. PRUEBA AISLADA DE AUTORIZACIÓN
// ============================================================

// Para probarla, cambiar temporalmente el servicio para que
// PoseeElPermiso devuelva false y ejecutar con:
// --probar-autorizacion

if (args.Contains("--probar-autorizacion"))
{
    Console.WriteLine("========== PRUEBA DE AUTORIZACIÓN ==========");
    try
    {
        agregarReclamo.Ejecutar(new AgregarReclamoRequest("Reclamo para probar autorización", idUsuario));
        Console.WriteLine("La operación fue autorizada. Revisá el servicio provisional.");
    }
    catch (DominioException ex)
    {
        MostrarError(ex);
    }
    catch (AutorizacionException ex)
    {
        Console.WriteLine(
            $"Autorización rechazada correctamente: {ex.Message}");
    }
    catch (Exception ex)
    {
        MostrarError(ex);
    }

    return;
}

// ============================================================
// 6. CAMINO FELIZ
// ============================================================

Console.WriteLine();
Console.WriteLine("========== CAMINO FELIZ ==========");
try
{
    // 6.1. Creamos el reclamo
    var responseAlta = agregarReclamo.Ejecutar(new AgregarReclamoRequest("Reparación de luminaria en la vía pública", idUsuario));
    var idReclamo = responseAlta.Id;
    Console.WriteLine($"Reclamo creado: {idReclamo}");
    MostrarEstado(idReclamo, "Estado inicial");

    // 6.2. Agregamos una inspección
    agregarActuacion.Ejecutar(new AgregarActuacionRequest("Se inspeccionó la luminaria.", idUsuario, TipoActuacion.Inspeccion, idReclamo));
    MostrarEstado(idReclamo, "Después de Inspeccion");

    // 6.3. Agregamos una orden de trabajo
    agregarActuacion.Ejecutar(new AgregarActuacionRequest("Se generó una orden de trabajo.", idUsuario, TipoActuacion.OrdenDeTrabajo, idReclamo));
    MostrarEstado(idReclamo, "Después de OrdenDeTrabajo");

    // 6.4. Agregamos el trabajo realizado
    var responseTrabajo = agregarActuacion.Ejecutar(new AgregarActuacionRequest("Se reemplazó la luminaria.", idUsuario, TipoActuacion.TrabajoRealizado, idReclamo));
    var idTrabajoRealizado = responseTrabajo.Id;
    MostrarEstado(idReclamo, "Después de TrabajoRealizado");
    // 6.5. Agregamos una observación.
    // No debe cambiar el estado.
    agregarActuacion.Ejecutar(new AgregarActuacionRequest("Se deja constancia del trabajo realizado.", idUsuario, TipoActuacion.Observacion, idReclamo));
    MostrarEstado(idReclamo, "Después de Observacion");

    // Guardamos las actuaciones actuales antes de eliminar la última
    var actuacionesRegistradas = actuacionRepository.ObtenerReclamoId(idReclamo).ToList();
    var ultimaActuacion = actuacionesRegistradas.LastOrDefault();

    // Definimos las actuaciones esperadas (todas menos la última que vamos a eliminar)
    var actuacionesEsperadas = actuacionesRegistradas
        .Where(a => ultimaActuacion is null || a.id != ultimaActuacion.id)
        .ToList();

    // 6.6. Eliminamos únicamente la última actuación (la Observación)
    if (ultimaActuacion is not null)
    {
        eliminarActuacion.Ejecutar(new EliminarActuacionRequest(ultimaActuacion.id, idUsuario));
        Console.WriteLine($"Se eliminó la última actuación con ID: {ultimaActuacion.id} ({ultimaActuacion.tipo})");
    }
    MostrarEstado(idReclamo, "Después de eliminar la última actuación");

    // 6.7. Modificamos el asunto
    modificarAsunto.Ejecutar(new ModificarAsuntoReclamoRequest(idReclamo, "Luminaria reparada en la vía pública", idUsuario));
    Console.WriteLine("Asunto del reclamo modificado.");

    // 6.8. Cambiamos manualmente el estado
    cambiarEstado.Ejecutar(new CambiarEstadoReclamoRequest(idReclamo, EstadoReclamo.EnInspeccion, idUsuario));
    MostrarEstado(idReclamo, "Después del cambio manual");

    // 6.9. Listamos los reclamos
    Console.WriteLine();
    Console.WriteLine("--- Reclamos registrados ---");
    var responseReclamos = listarReclamos.Ejecutar(new ListarReclamosRequest());
    foreach (var reclamo in responseReclamos.Reclamos)
    {
        Console.WriteLine($"ID: {reclamo.Id} | Asunto: {reclamo.Asunto} | Estado: {reclamo.Estado}");
    }

   // 6.10. Listamos Actuaciones Esperadas y Actuaciones Devueltas
    Console.WriteLine();
    Console.WriteLine("--- Actuaciones Esperadas ---");
    foreach (var act in actuacionesEsperadas)
    {
        Console.WriteLine($"ID: {act.id} | Tipo: {act.tipo} | Detalle: {act.detalle}");
    }

    Console.WriteLine();
    Console.WriteLine("--- Actuaciones Devueltas ---");
    var responseActuaciones = listarActuaciones.Ejecutar(new ListarActuacionPorReclamoRequest(idReclamo, idUsuario));
    foreach (var actuacion in responseActuaciones.actuaciones)
    {
        Console.WriteLine($"ID: {actuacion.Id} | Tipo: {actuacion.Tipo} | Detalle: {actuacion.Detalle}");
    }

    // 6.11. Probamos la eliminación en cascada.
    // Agregamos una actuación para comprobar que también se elimine.
    agregarActuacion.Ejecutar(new AgregarActuacionRequest("Actuación de prueba para eliminación en cascada.", idUsuario, TipoActuacion.Observacion, idReclamo));
    eliminarReclamo.Ejecutar(new EliminarReclamoRequest(idReclamo, idUsuario));
    var reclamoEliminado = reclamoRepository.ObtenerPorId(idReclamo);
    var quedanActuaciones = actuacionRepository.ObtenerReclamoId(idReclamo).Any();

    Console.WriteLine();
    Console.WriteLine($"Reclamo eliminado: {reclamoEliminado is null}");
    Console.WriteLine($"Quedan actuaciones asociadas: {quedanActuaciones}");
}
catch (DominioException ex)
{
    MostrarError(ex);
}
catch (AutorizacionException ex)
{
    MostrarError(ex);
}
catch (Exception ex)
{
    MostrarError(ex);
}

// ============================================================
// 7. CAMINOS DE ERROR
// ============================================================

Console.WriteLine();
Console.WriteLine("========== CAMINOS DE ERROR ==========");

// 7.1. Asunto vacío

try
{
    agregarReclamo.Ejecutar(new AgregarReclamoRequest("   ", idUsuario));

    Console.WriteLine("ERROR: se aceptó un asunto vacío.");
}
catch (DominioException ex)
{
    Console.WriteLine($"Asunto vacío rechazado: {ex.Message}");
}
catch (AutorizacionException ex)
{
    MostrarError(ex);
}
catch (Exception ex)
{
    MostrarError(ex);
}

// 7.2. Asunto de más de 200 caracteres
try
{
    agregarReclamo.Ejecutar(new AgregarReclamoRequest(new string('A', 201),idUsuario));
    Console.WriteLine("ERROR: se aceptó un asunto demasiado largo.");
}
catch (DominioException ex)
{
    Console.WriteLine($"Asunto demasiado largo rechazado: {ex.Message}");
}
catch (AutorizacionException ex)
{
    MostrarError(ex);
}
catch (Exception ex)
{
    MostrarError(ex);
}

// 7.3. Actuación para un reclamo inexistente

try
{
    agregarActuacion.Ejecutar(new AgregarActuacionRequest("Actuación para un reclamo inexistente.", idUsuario, TipoActuacion.Inspeccion, Guid.NewGuid()));
    Console.WriteLine("ERROR: se aceptó un reclamo inexistente.");
}
catch (DominioException ex)
{
    MostrarError(ex);
}
catch (AutorizacionException ex)
{
    MostrarError(ex);
}
catch (Exception ex)
{
    Console.WriteLine($"No se pudo agregar la actuación: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("========== FIN DE LAS PRUEBAS ==========");
