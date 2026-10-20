
using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Reclamos;
using SGR.Dominio.Comun;
using SGR.Infraestructura.Autorizacion;
using SGR.Infraestructura.Reclamos;

// Prepararmos el archivo y los repositorios
var rutaArchivo = Path.Combine(Environment.CurrentDirectory, "Datos", "reclamos.txt");

Directory.CreateDirectory(Path.GetDirectoryName(rutaArchivo)!);

IReclamoRepository reclamoRepository = new ReclamoTxtRepository(rutaArchivo);
IAutorizacionService autorizacionService = new AutorizacionProvisionalService();

// Creamos los casos de uso
var agregarReclamo = new AgregarReclamoUseCase(reclamoRepository, autorizacionService);
var obtenerReclamo = new ObtenerReclamoPorIdUseCase(reclamoRepository);
var listarReclamos = new ListarReclamosUseCase(reclamoRepository);
var modificarAsunto = new ModificarAsuntoReclamoUseCase(reclamoRepository, autorizacionService);
var cambiarEstado = new CambiarEstadoReclamoUseCase(reclamoRepository, autorizacionService);

// Creamos cinco reclamos y guardarmos sus ID
var idsReclamos = new List<Guid>();
var idUsuario = Guid.NewGuid();

Console.WriteLine("=== ALTA DE RECLAMOS ===");

for (var i = 1; i <= 5; i++)
{
    var requestAlta = new AgregarReclamoRequest($"Reclamo de prueba {i}", idUsuario);
    var responseAlta = agregarReclamo.Ejecutar(requestAlta);

    idsReclamos.Add(responseAlta.Id);

    Console.WriteLine($"Reclamo {i} creado con ID: {responseAlta.Id}");
}

// Usaremos el primer reclamo para las modificaciones y la baja
var idReclamoPrueba = idsReclamos[0];

//  Modificar el asunto del primer reclamo
Console.WriteLine("\n=== MODIFICAR ASUNTO ===");

var requestModificar = new ModificarAsuntoReclamoRequest(
    idReclamoPrueba,
    "Bache en la avenida principal",
    idUsuario
);

modificarAsunto.Ejecutar(requestModificar);

Console.WriteLine("Asunto modificado correctamente.");

// Cambiamos el estado del primer reclamo
Console.WriteLine("\n=== CAMBIAR ESTADO ===");

var requestCambiarEstado = new CambiarEstadoReclamoRequest(
    idReclamoPrueba,
    EstadoReclamo.EnInspeccion,
    idUsuario
);

cambiarEstado.Ejecutar(requestCambiarEstado);

Console.WriteLine("Estado modificado correctamente.");

// 6. Obtener el primer reclamo y verificar las modificaciones.
Console.WriteLine("\n=== VERIFICAR MODIFICACIONES ===");

var requestObtener = new ObtenerReclamoPorIdRequest(idReclamoPrueba);
var responseObtener = obtenerReclamo.Ejecutar(requestObtener);

Console.WriteLine($"ID: {responseObtener.Reclamo.Id}");
Console.WriteLine($"Asunto: {responseObtener.Reclamo.Asunto}");
Console.WriteLine($"Estado: {responseObtener.Reclamo.Estado}");

// 7. Listar todos los reclamos.
Console.WriteLine("\n=== LISTADO ANTES DE LA BAJA ===");

var responseListado = listarReclamos.Ejecutar(new ListarReclamosRequest());

foreach (var reclamo in responseListado.Reclamos)
{
    Console.WriteLine($"ID: {reclamo.Id} | Asunto: {reclamo.Asunto} | Estado: {reclamo.Estado}");
}