using SGR.Aplicacion;
using SGR.Aplicacion.Abstracciones;
using System.Security.Cryptography;
using System.Text;
namespace SGR.Infraestructura;
public class HashService : IHashService
{
    
    public String Hashear (String contraseña)
    {
        // 1. Convertir el string a arreglo de bytes usando UTF-8 explícito
        byte[] bytesTexto = Encoding.UTF8.GetBytes(contraseña);

        // 2. Calcular el hash SHA-256 (devuelve un byte[] de 32 bytes)
        byte[] bytesHash = SHA256.HashData(bytesTexto);

        // 3. Convertir los bytes del hash a string hexadecimal (64 caracteres)
        return Convert.ToHexString(bytesHash); // o .ToLowerInvariant() si lo prefieres en minúsculas
    }
}