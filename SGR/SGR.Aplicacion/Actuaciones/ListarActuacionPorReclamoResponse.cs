namespace SGR.Aplicacion.Actuaciones;
using SGR.Dominio.Actuaciones;

public record class ListarActuacionPorReclamoResponse ( IEnumerable<ActuacionDTO> actuaciones);