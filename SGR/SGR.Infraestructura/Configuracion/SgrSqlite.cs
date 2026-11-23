
using System.Data;
using Microsoft.EntityFrameworkCore;
using SGR.Dominio.Usuarios;
using SGR.Dominio.Reclamos;
using SGR.Dominio.Actuaciones;
using SGR.Infraestructura;
using SGR.Dominio.Comun;

namespace SGR.Infraestructura.Configuracion;

public static class SgrSqlite
{
    public static void Inicializar(SgrContext context)
    {
        // Crear la base de datos si todavía no existe.
        if (context.Database.EnsureCreated())
        {
            // Obtener la conexión con SQLite.
            var conexion = context.Database.GetDbConnection();

            // Abrir la conexión.
            if (conexion.State != ConnectionState.Open)
                conexion.Open();

            // Configurar el modo de registro de SQLite.
            using var comando = conexion.CreateCommand();
            comando.CommandText = "PRAGMA journal_mode=DELETE;";
            comando.ExecuteNonQuery();

            // Cerrar la conexión.
            conexion.Close();

            // Crear el servicio para calcular los hashes de las contraseñas.
            var hashService = new HashService();

            // Crear el administrador.
            var admin = new Usuario("Administrador", new CorreoElectronico("admin@sgr.com"), hashService.Hashear("admin1234"), true);

            // Crear un usuario con permisos parciales.
            var agente = new Usuario("Agente de prueba", new CorreoElectronico("agente@sgr.com"), hashService.Hashear("agente1234"), false);

            agente.AgregarPermiso(Permiso.ReclamoAlta);
            agente.AgregarPermiso(Permiso.ReclamoModificacion);
            agente.AgregarPermiso(Permiso.ActuacionAlta);

            // Crear un usuario sin permisos.
            var consultor = new Usuario("Consultor de prueba", new CorreoElectronico("consultor@sgr.com"), hashService.Hashear("consultor1234"), false);

            // Agregar los usuarios al contexto.
            context.Usuarios.Add(admin);
            context.Usuarios.Add(agente);
            context.Usuarios.Add(consultor);

            // Aquí cargaremos los reclamos y las actuaciones.
            // Crear reclamos de prueba.
            var reclamo1 = new Reclamo(new Asunto("Problema con el alumbrado público"), DateTime.Now, agente.Id);
            var reclamo2 = new Reclamo(new Asunto("Pérdida de agua en la vía pública"), DateTime.Now, admin.Id);
            var reclamo3 = new Reclamo(new Asunto("Acumulación de residuos en la vereda"), DateTime.Now, agente.Id);

            // Agregar los reclamos al contexto.
            context.Reclamos.Add(reclamo1);
            context.Reclamos.Add(reclamo2);
            context.Reclamos.Add(reclamo3); 

            // Crear actuaciones de prueba.
            var actuacion1 = new Actuacion( reclamo1.Id, TipoActuacion.Inspeccion, new Detalle("Se realizó una inspección del alumbrado público."), DateTime.Now, agente.Id);
            var actuacion2 = new Actuacion( reclamo1.Id, TipoActuacion.OrdenDeTrabajo, new Detalle("Se solicita reparar las luminarias afectadas."), DateTime.Now, agente.Id);
            var actuacion3 = new Actuacion( reclamo2.Id, TipoActuacion.Observacion, new Detalle("Se recibió el aviso de pérdida de agua."), DateTime.Now, admin.Id);
            var actuacion4 = new Actuacion( reclamo3.Id, TipoActuacion.TrabajoRealizado, new Detalle("Se retiraron los residuos acumulados."), DateTime.Now, agente.Id);

            // Actualizar el estado de los reclamos según sus actuaciones.
            reclamo1.ActualizarEstado(TipoActuacion.OrdenDeTrabajo, agente.Id);
            reclamo2.ActualizarEstado(TipoActuacion.Observacion, admin.Id);
            reclamo3.ActualizarEstado(TipoActuacion.TrabajoRealizado, agente.Id);

            context.Actuaciones.Add(actuacion1);
            context.Actuaciones.Add(actuacion2);
            context.Actuaciones.Add(actuacion3);
            context.Actuaciones.Add(actuacion4);

            context.SaveChanges();
        }
    }
}
