using SGR.Aplicacion.Autorizaciones;
using SGR.Aplicacion.Reclamos;
using SGR.Infraestructura.Autorizacion;
using SGR.Infraestructura.Reclamos;

var rutaArchivo = Path.Combine(Environment.CurrentDirectory, "Datos", "reclamos.txt");

Directory.CreateDirectory(Path.GetDirectoryName(rutaArchivo)!);

IReclamoRepository reclamoRepository = new ReclamoTxtRepository(rutaArchivo);
IAutorizacionService autorizacionService = new AutorizacionProvisionalService();

var agregarReclamo = new AgregarReclamoUseCase(reclamoRepository, autorizacionService);

var request = new AgregarReclamoRequest("Bache en la calle principal", Guid.NewGuid());

var response = agregarReclamo.Ejecutar(request);

Console.WriteLine($"Reclamo creado con Id: {response.Id}");