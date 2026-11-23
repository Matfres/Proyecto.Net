using SGR.Dominio.Usuarios;

namespace SGR.Aplicacion.Abstracciones;

public interface IUsuarioRepository : IRepository<Usuario>
{
    // Agregamos este método específico
    Usuario? ObtenerPorEmail(CorreoElectronico email);
}