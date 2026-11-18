
using System.Data;
using Microsoft.EntityFrameworkCore;
using SGR.Infraestructura.Datos;

namespace SGR.Infraestructura.Configuracion;

public static class SgrSqlite
{
    public static void Inicializar(SgrContext context)
    {
        // Crear la base de datos si todavía no existe.
        context.Database.EnsureCreated();

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
    }
}
