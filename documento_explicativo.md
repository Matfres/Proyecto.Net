# SGR --- Guía de pruebas de la aplicación

## 1. Objetivo

Este documento explica cómo ejecutar y comprobar las funcionalidades implementadas en el sistema de Gestión de Reclamos (SGR). Las pruebas se realizan directamente desde el punto de entrada de la aplicación (`Program.cs`), el cual se encarga de realizar la composición de dependencias y coordinar la ejecución de los escenarios de prueba.

Se cubren la prueba aislada de autorización, el flujo principal (camino feliz), la evolución y recálculo de estados según las actuaciones, la modificación manual de datos y la validación de los caminos de error exigidos por el dominio.

> **Nota:** Los identificadores (`Guid`) generados durante las distintas ejecuciones son dinámicos, por lo que los valores concretos cambiarán en cada ejecución. En este documento se muestran salidas con referencias con `<Guid>` como guía.

---

## 2. Preparación y ejecución

La solución compone sus dependencias e inicializa los recursos necesarios de la siguiente manera:

1. **Directorio de Persistencia:** Crea el directorio local `./Datos` si no existe previamente.
2. **Repositorios en Archivo TXT:** Instancia `ReclamoTxtRepository` y `ActuacionesTxtRepository` apuntando a `Datos/reclamos.txt` y `Datos/actuaciones.txt` respectivamente.
3. **Servicios de Infraestructura e Inyección:** Crea `AutorizacionProvisionalService` y `ActualizacionEstadoReclamoService`, e instanciar todos los casos de uso de Reclamos y Actuaciones.
> **Nota:** Para el guardado y la recuperación de las entidades se decidio  realizar un proceso de codificacion sobre su asunto o detalle para evitar errores respecto al separador de campos dentro de los repositorios. Cada operacion dentro de los repositorios requieren codificar o descodificar estos campos, esto es una accion interna.
### Ejecución del flujo principal
Antes de inicar el programa se necesita situarse en la carpeta /SGR/SGR.Consola.Ya ubicado dentro de las carpetas,desde la consola de comandos, ejecute:

```bash
dotnet run 
```

Esto correrá secuencialmente el **Camino Feliz** y los **Caminos de Error**. Los archivos de datos `.txt` persistirán el estado entre ejecuciones dentro del directorio `Datos`.

### Ejecución de la prueba de autorización

Para probar específicamente el escenario aislado de control de permisos:

```bash
dotnet run -- --probar-autorizacion
```

> **Configuración previa de autorización:** Para comprobar la denegación como tal, configure temporalmente el servicio provisional (`AutorizacionProvisionalService`) para que `PoseeElPermiso` devuelva `false`. Una vez terminada esa verificación, restaure el comportamiento habitual (`return true;`) para ejecutar el flujo normal.

---

## 3. Prueba aislada de autorización

El script evalúa inicialmente si el usuario cuenta con los permisos necesarios para realizar operaciones sobre los casos de uso.

### Código de ejemplo (`Program.cs`)

```csharp
if (args.Contains("--probar-autorizacion"))
{
    Console.WriteLine("========== PRUEBA DE AUTORIZACIÓN ==========");
    try
    {
        agregarReclamo.Ejecutar(new AgregarReclamoRequest("Reclamo para probar autorización", idUsuario));
        Console.WriteLine("La operación fue autorizada. Revisá el servicio provisional.");
    }
    catch (DominioException ex)
    {
        MostrarError(ex);
    }
    catch (AutorizacionException ex)
    {
        Console.WriteLine(
            $"Autorización rechazada correctamente: {ex.Message}");
    }
    catch (Exception ex)
    {
        MostrarError(ex);
    }

    return;
}
```

### Salida esperada por consola

*   **Cuando el servicio autoriza la operación (`return true`):**

```text
========== PRUEBA DE AUTORIZACIÓN ==========
La operación fue autorizada. Revisá el servicio provisional.
```

*   **Cuando el servicio deniega la operación (`return false`):**

```text
========== PRUEBA DE AUTORIZACIÓN ==========
Autorización rechazada correctamente: El usuario no posee permisos para realizar esta operación
```

---

## 4. Flujo principal: Camino feliz

En el flujo principal se crea un reclamo, se registran actuaciones secuenciales comprobando la transición automática de estados, se modifica información básica y finalmente se consulta e inspecciona el estado del sistema.

### 4.1. Creación del reclamo y evolución de estados

El reclamo inicia en su estado primario. Al agregar actuaciones, el servicio `ActualizacionEstadoReclamoService` recalcula el estado según la naturaleza de la actuación (`Inspeccion`, `OrdenDeTrabajo`, `TrabajoRealizado`). Las actuaciones de tipo `Observacion` se agregan al historial pero no provocan cambios en el estado del reclamo.

#### Código de ejemplo (`Program.cs`)

```csharp
// 6.1. Creamos el reclamo
var responseAlta = agregarReclamo.Ejecutar(new AgregarReclamoRequest("Reparación de luminaria en la vía pública", idUsuario));
var idReclamo = responseAlta.Id;
Console.WriteLine($"Reclamo creado: {idReclamo}");
MostrarEstado(idReclamo, "Estado inicial");

// 6.2. Agregamos una inspección
agregarActuacion.Ejecutar(new AgregarActuacionRequest("Se inspeccionó la luminaria.", idUsuario, TipoActuacion.Inspeccion, idReclamo));
MostrarEstado(idReclamo, "Después de Inspeccion");

// 6.3. Agregamos una orden de trabajo
agregarActuacion.Ejecutar(new AgregarActuacionRequest("Se generó una orden de trabajo.", idUsuario, TipoActuacion.OrdenDeTrabajo, idReclamo));
MostrarEstado(idReclamo, "Después de OrdenDeTrabajo");

// 6.4. Agregamos el trabajo realizado
var responseTrabajo = agregarActuacion.Ejecutar(new AgregarActuacionRequest("Se reemplazó la luminaria.", idUsuario, TipoActuacion.TrabajoRealizado, idReclamo));
var idTrabajoRealizado = responseTrabajo.Id;
MostrarEstado(idReclamo, "Después de TrabajoRealizado");

// 6.5. Agregamos una observación (No altera el estado)
agregarActuacion.Ejecutar(new AgregarActuacionRequest("Se deja constancia del trabajo realizado.", idUsuario, TipoActuacion.Observacion, idReclamo));
MostrarEstado(idReclamo, "Después de Observacion");
```

#### Salida esperada por consola

```text
========== CAMINO FELIZ ==========
Reclamo creado: e4d9b012-7f28-4171-893a-8b2c451001a1
Estado inicial: Creado
Después de Inspeccion: EnInspeccion
Después de OrdenDeTrabajo: EnPlanificacion
Después de TrabajoRealizado: Finalizado
Después de Observacion: Finalizado
```

---

## 5. Eliminación de actuaciones, modificación y listado

### 5.1. Eliminación de actuaciones individuales

El programa identifica la última actuación registrada (en este caso, la `Observacion`) y procede a su eliminación mediante `EliminarActuacionUseCase`, verificando que el historial de actuaciones devuelto coincida exactamente con las actuaciones esperadas tras el borrado.

#### Código de ejemplo (`Program.cs`)

```csharp
var actuacionesRegistradas = actuacionRepository.ObtenerReclamoId(idReclamo).ToList();
var ultimaActuacion = actuacionesRegistradas.LastOrDefault();

var actuacionesEsperadas = actuacionesRegistradas
    .Where(a => ultimaActuacion is null || a.id != ultimaActuacion.id)
    .ToList();

if (ultimaActuacion is not null)
{
    eliminarActuacion.Ejecutar(new EliminarActuacionRequest(ultimaActuacion.id, idUsuario));
    Console.WriteLine($"Se eliminó la última actuación con ID: {ultimaActuacion.id} ({ultimaActuacion.tipo})");
}
MostrarEstado(idReclamo, "Después de eliminar la última actuación");
```

### 5.2. Modificación de asunto, estado manual y listados

Se comprueba la edición del asunto, la modificación manual directa sobre el estado del reclamo y la consulta de reclamos y actuaciones registradas a través de los casos de uso de lectura.

#### Código de ejemplo (`Program.cs`)

```csharp
// Modificación de asunto y estado manual
modificarAsunto.Ejecutar(new ModificarAsuntoReclamoRequest(idReclamo, "Luminaria reparada en la vía pública", idUsuario));
Console.WriteLine("Asunto del reclamo modificado.");

cambiarEstado.Ejecutar(new CambiarEstadoReclamoRequest(idReclamo, EstadoReclamo.EnInspeccion, idUsuario));
MostrarEstado(idReclamo, "Después del cambio manual");

// Listado de Reclamos
Console.WriteLine();
Console.WriteLine("--- Reclamos registrados ---");
var responseReclamos = listarReclamos.Ejecutar(new ListarReclamosRequest());
foreach (var reclamo in responseReclamos.Reclamos)
{
    Console.WriteLine($"ID: {reclamo.Id} | Asunto: {reclamo.Asunto} | Estado: {reclamo.Estado}");
}

// Listado de Actuaciones Esperadas vs. Devueltas
Console.WriteLine();
Console.WriteLine("--- Actuaciones Esperadas ---");
foreach (var act in actuacionesEsperadas)
{
    Console.WriteLine($"ID: {act.id} | Tipo: {act.tipo} | Detalle: {act.detalle}");
}

Console.WriteLine();
Console.WriteLine("--- Actuaciones Devueltas ---");
var responseActuaciones = listarActuaciones.Ejecutar(new ListarActuacionPorReclamoRequest(idReclamo, idUsuario));
foreach (var actuacion in responseActuaciones.actuaciones)
{
    Console.WriteLine($"ID: {actuacion.Id} | Tipo: {actuacion.Tipo} | Detalle: {actuacion.Detalle}");
}
```

#### Salida esperada por consola

```text
Se eliminó la última actuación con ID: 3a1b8c2d-90ef-41a2-b345-6789abcdef01 (Observacion)
Después de eliminar la última actuación: Finalizado
Asunto del reclamo modificado.
Después del cambio manual: EnInspeccion

--- Reclamos registrados ---
ID: <Guid> | Asunto: Luminaria reparada en la vía pública | Estado: EnInspeccion

--- Actuaciones Esperadas ---
ID: <Guid>| Tipo: Inspeccion | Detalle: Se inspeccionó la luminaria.
ID: <Guid> | Tipo: OrdenDeTrabajo | Detalle: Se generó una orden de trabajo.
ID: <Guid> | Tipo: TrabajoRealizado | Detalle: Se reemplazó la luminaria.

--- Actuaciones Devueltas ---
ID: <Guid> | Tipo: Inspeccion | Detalle: Se inspeccionó la luminaria.
ID: <Guid> | Tipo: OrdenDeTrabajo | Detalle: Se generó una orden de trabajo.
ID: <Guid> | Tipo: TrabajoRealizado | Detalle: Se reemplazó la luminaria.
```

---

## 6. Eliminación del reclamo en cascada

Para garantizar la integridad y evitar registros de actuaciones sin un reclamo a corresponder en los archivos TXT, la eliminación de un reclamo remueve automáticamente todas las actuaciones vinculadas a su `Guid`.

### Código de ejemplo (`Program.cs`)

```csharp
agregarActuacion.Ejecutar(new AgregarActuacionRequest("Actuación de prueba para eliminación en cascada.", idUsuario, TipoActuacion.Observacion, idReclamo));

eliminarReclamo.Ejecutar(new EliminarReclamoRequest(idReclamo, idUsuario));

var reclamoEliminado = reclamoRepository.ObtenerPorId(idReclamo);
var quedanActuaciones = actuacionRepository.ObtenerReclamoId(idReclamo).Any();

Console.WriteLine();
Console.WriteLine($"Reclamo eliminado: {reclamoEliminado is null}");
Console.WriteLine($"Quedan actuaciones asociadas: {quedanActuaciones}");
```

### Salida esperada por consola

```text
Reclamo eliminado: True
Quedan actuaciones asociadas: False
```

---

## 7. Caminos de error

El sistema valida que las reglas de negocio del dominio se cumplan, rechazando peticiones inválidas mediante la captura de `DominioException` u otras excepciones controladas.

### 7.1. Asunto vacío

Se intenta registrar un reclamo con espacios en blanco como asunto.

```csharp
try
{
    agregarReclamo.Ejecutar(new AgregarReclamoRequest("   ", idUsuario));
    Console.WriteLine("ERROR: se aceptó un asunto vacío.");
}
catch (DominioException ex)
{
    Console.WriteLine($"Asunto vacío rechazado: {ex.Message}");
}
```

**Salida esperada:**
```text
Asunto vacío rechazado: El asunto del reclamo no puede estar vacío.
```

### 7.2. Asunto de más de 200 caracteres

Se intenta registrar un reclamo cuya longitud superas la regla de dominio (200 caracteres máximo).

```csharp
try
{
    agregarReclamo.Ejecutar(new AgregarReclamoRequest(new string('A', 201), idUsuario));
    Console.WriteLine("ERROR: se aceptó un asunto demasiado largo.");
}
catch (DominioException ex)
{
    Console.WriteLine($"Asunto demasiado largo rechazado: {ex.Message}");
}
```

**Salida esperada:**
```text
Asunto demasiado largo rechazado: El asunto no puede superar los 200 caracteres.
```

### 7.3. Actuación para un reclamo inexistente

Se intenta ingresar una actuación utilizando un identificador `Guid` aleatorio que no está asociado a ningún reclamo persistido.

```csharp
try
{
    agregarActuacion.Ejecutar(new AgregarActuacionRequest("Actuación para un reclamo inexistente.", idUsuario, TipoActuacion.Inspeccion, Guid.NewGuid()));
    Console.WriteLine("ERROR: se aceptó un reclamo inexistente.");
}
catch (Exception ex)
{
    Console.WriteLine($"No se pudo agregar la actuación: {ex.Message}");
}
```

**Salida esperada:**
```text
No se pudo agregar la actuación: No se encontró el reclamo especificado.
```

---

## 8. Resumen de resultados esperados

Al ejecutar el comando `dotnet run` sin parámetros, la salida consolidada en la consola debe ser la siguiente:

```text
========== CAMINO FELIZ ==========
Reclamo creado: <Guid_Reclamo>
Estado inicial: Creado
Después de Inspeccion: EnInspeccion
Después de OrdenDeTrabajo: EnPlanificacion
Después de TrabajoRealizado: Finalizado
Después de Observacion: Finalizado
Se eliminó la última actuación con ID: <Guid_Actuacion> (Observacion)
Después de eliminar la última actuación: Finalizado
Asunto del reclamo modificado.
Después del cambio manual: EnInspeccion

--- Reclamos registrados ---
ID: <Guid> | Asunto: Luminaria reparada en la vía pública | Estado: EnInspeccion

--- Actuaciones Esperadas ---
ID: <Guid> | Tipo: Inspeccion | Detalle: Se inspeccionó la luminaria.
ID: <Guid> | Tipo: OrdenDeTrabajo | Detalle: Se generó una orden de trabajo.
ID: <Guid> | Tipo: TrabajoRealizado | Detalle: Se reemplazó la luminaria.

--- Actuaciones Devueltas ---
ID: <Guid> | Tipo: Inspeccion | Detalle: Se inspeccionó la luminaria.
ID: <Guid> | Tipo: OrdenDeTrabajo | Detalle: Se generó una orden de trabajo.
ID: <Guid> | Tipo: TrabajoRealizado | Detalle: Se reemplazó la luminaria.

Reclamo eliminado: True
Quedan actuaciones asociadas: False

========== CAMINOS DE ERROR ==========
Asunto vacío rechazado: El asunto del reclamo no puede estar vacío.
Asunto demasiado largo rechazado: El asunto no puede superar los 200 caracteres.
No se pudo agregar la actuación: No se encontró el reclamo especificado.

========== FIN DE LAS PRUEBAS ==========
```

---

## 9. Consideraciones finales

- **Persistencia en Disco:** Las pruebas generan datos en `./Datos/reclamos.txt` y `./Datos/actuaciones.txt`. Si se desea iniciar las pruebas desde un estado completamente limpio, elimine la carpeta `Datos` antes de ejecutar.
- **Identificadores Dinámicos:** Los identificadores de usuarios, reclamos y actuaciones son generados con `Guid.NewGuid()`, asegurando ejecuciones independientes.
- **Eliminación en Cascada:** Se verifica la consistencia entre los repositorios de reclamos y actuaciones al momento de dar de baja una entidad principal.