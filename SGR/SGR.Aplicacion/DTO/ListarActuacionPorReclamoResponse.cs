namespace SGR.Aplicacion.DTO;
using SGR.Dominio.Actuaciones;

public record class ListarActuacionPorReclamoResponse ( IEnumerable<ActuacionDTO> actuaciones);