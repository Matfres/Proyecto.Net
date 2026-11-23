using SGR.Dominio.Usuarios;
using SGR.Infraestructura.Configuracion;
using SGR.Aplicacion.Abstracciones;
namespace SGR.Infraestructura.Repositorios;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(SgrContext context) : base(context) { }

    public Usuario? ObtenerPorEmail(CorreoElectronico correoElectronico)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.CorreoElectronico.Valor == correoElectronico.Valor);
        
        return usuario;
    }
    
}
