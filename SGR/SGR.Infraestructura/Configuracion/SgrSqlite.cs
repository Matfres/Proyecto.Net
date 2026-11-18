
using System.Data;
using Microsoft.EntityFrameworkCore;
using SGR.Dominio.Usuarios;
using SGR.Dominio.Reclamos;
using SGR.Dominio.Actuaciones;
using SGR.Infraestructura;

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
        }
    }
}
