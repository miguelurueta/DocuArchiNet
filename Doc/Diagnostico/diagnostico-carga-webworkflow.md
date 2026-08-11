# Diagnóstico de carga de Webworkflow

Fecha: 2026-08-04  
Página analizada: `workflow/Webworkflow.aspx`  
Code-behind: `workflow/Webworkflow.aspx.vb`

## Síntoma

La apertura del listado de tareas Workflow puede tardar entre 7 y 45 segundos. La percepción inicial era que toda la demora pertenecía a `Page_Load` y a la consulta principal de tareas.

## Recorrido de la solicitud

La navegación no consiste únicamente en ejecutar `Webworkflow.Page_Load`:

```text
Clic del usuario
  -> navegación/postback de la página contenedora
  -> solicitud GET de Webworkflow.aspx
  -> pipeline ASP.NET
  -> adquisición de Session
  -> PreInit / Init
  -> Page_Load
  -> consulta y DataBind
  -> renderizado y descarga de recursos
```

`Webworkflow.aspx` tiene aproximadamente 387 KB y 4.648 líneas, por lo que la compilación en frío y construcción de controles también tiene un costo, aunque no resultó ser el cuello principal medido.

## Medición del Page_Load

Se instrumentaron con `Stopwatch` las funciones principales. Una ejecución local produjo:

```text
Retorna_nombre_ruta_por_id_ruta                 1 ms
Determina_Tipo_Actividad_Usuario                2 ms
Inicializar_la_lista_de_tareas_workflow       258 ms
Verifica_Tarea_Seleccionada_Uusario_Inicio      4 ms
Intervalo_Alarma_Usuario                        1 ms
Inicializa_firma_usuario_workflow              20 ms
Firma_transparente                              2 ms
TOTAL_PAGE_LOAD                               299 ms
```

Conclusión: el `Page_Load` no explica una espera inicial cercana a siete segundos en el entorno analizado.

## Medición del pipeline ASP.NET

Se instrumentaron `Global.asax`, `PreInit`, `Init`, `Page_Load` y `Unload`:

```text
BeginRequest                   6 ms
AuthorizeRequest              11 ms
AcquireRequestState        6.144 ms
PreInit                    6.605 ms
Page_Load                  6.611 ms
Unload                     7.037 ms
EndRequest                 7.037 ms
```

El 87% aproximado de la espera ocurrió antes de que la página adquiriera la sesión. La solicitud Workflow permaneció unos 6,1 segundos esperando `Session`.

## Causa raíz: contención de Session

Durante la espera se observaron múltiples solicitudes con el mismo `SessionID`:

```text
Handler_lista_numero_solicitudes_dbase.ashx
Handler_Lista_compartidos_por_revision_db.ashx
Handler_lista_tramites_wf_asignados_db.ashx
Handler_lista_tareas_asignadas_workflow_db.ashx
Handler_lista_tareas_asignadas_workflow.ashx
```

Los handlers implementan `IRequiresSessionState`, que solicita acceso exclusivo a la sesión. ASP.NET serializa las solicitudes del mismo usuario: mientras una conserva el bloqueo, las restantes esperan antes de ejecutar su handler o página.

Adicionalmente, `Global.asax.vb` fuerza este comportamiento globalmente:

```vb
HttpContext.Current.SetSessionStateBehavior(
    System.Web.SessionState.SessionStateBehavior.Required)
```

La página inicial dispara consultas AJAX concurrentes. Una de ellas se repite cada 1.600 ms:

```javascript
setInterval(
    'remplaza_datos_lista_tareas_asignadas_workflow(...)',
    '1600'
);
```

Esto genera competencia continua por el bloqueo exclusivo de sesión, incluso para el handler que únicamente devuelve un valor almacenado en `Session`.

## Consulta principal e índice

La tabla de producción `estados_tarea_workflow` tiene aproximadamente 1.819.801 registros. Antes del índice, el plan de una actividad utilizaba:

```text
type: index_merge
rows: 23469
Extra: Using intersect(...); Using where; Using filesort
```

Se creó y validó localmente el índice:

```sql
ALTER TABLE estados_tarea_workflow
ADD INDEX IX_ETW_GRUPO_PENDIENTES
(
    Id_Actividad,
    Id_Usuario,
    Fecha_Fin,
    Fecha_Seleccion,
    ESTADO_ACTIVIDA_MODULO_RAD,
    Fecha_Inicio
);
```

Después del índice:

```text
type: ref
key: IX_ETW_GRUPO_PENDIENTES
rows: 698
Extra: Using where
```

Desaparecieron `index_merge` y `Using filesort`. El listado local quedó alrededor de 90-277 ms. El índice corrige el plan SQL, pero no elimina la espera por el bloqueo de sesión.

## Volumen y calidad de datos

Se encontró una actividad con 450.646 tareas abiertas, de las cuales 450.644 tenían `Id_Usuario IS NULL`. Esto indica tareas de grupo posiblemente abandonadas o nunca finalizadas.

Archivar tareas terminadas anteriores a dos años puede reducir el volumen operativo:

```sql
Fecha_Fin IS NOT NULL
AND Fecha_Fin < DATE_SUB(CURDATE(), INTERVAL 2 YEAR)
```

Esta regla no mueve las tareas problemáticas con `Fecha_Fin IS NULL`. Es necesario definir funcionalmente cuándo una tarea abierta puede considerarse obsoleta.

## Problemas secundarios encontrados

### Firma de usuario

La carga intenta descargar un BLOB, escribir un BMP y procesarlo en cada primera carga. En local se encontró:

```text
System.UnauthorizedAccessException
Acceso denegado a tempfirma/REGISTROPUBLICO/{usuario}/{usuario}.bmp
```

Debe revisarse el permiso de escritura de la identidad del Application Pool. La función de transparencia se ejecuta incluso cuando la inicialización de la firma falla.

### Excepciones ocultas del Grid

Durante `DataBind` se producen dos `ArgumentOutOfRangeException` de `System.Web.dll`. Existen bloques `Catch` vacíos, por lo que no se conoce la línea sin configurar Visual Studio para detenerse al lanzar la excepción. Probablemente se accede a celdas por índice antes de que todas existan.

### JavaScript duplicado

`JSProgresBar.js` está incluido dos veces en `Webworkflow.aspx`, produciendo:

```text
Identifier 'JSProgresBar' has already been declared
```

También se cargan dos versiones distintas de jQuery UI y numerosos recursos bloqueantes y externos.

### Procesamiento redundante

- El listado recupera hasta 2.000 registros aunque el Grid muestra 7.
- El `DataSet` completo se guarda en `Session` por usuario.
- El Grid ejecuta `DataBind` más de una vez.
- `SolicitaNumeroActividadesSelecionadasUsuario` usaba `SELECT *` solo para contar; su llamada fue encontrada comentada durante el análisis.
- `Obtener_Id_Tarea_Selecionada` no usa `LIMIT 1` aunque solo consume la primera fila.

## Recomendaciones priorizadas

1. Revisar y retirar el `SessionStateBehavior.Required` global, validando primero qué endpoints realmente necesitan escribir en sesión.
2. Cambiar handlers de solo lectura a `IReadOnlySessionState`.
3. Evitar que handlers de conteo escriban resultados temporales en `Session`; devolver valores locales en la respuesta.
4. Cancelar el polling al abandonar la pantalla que lo creó y aumentar el intervalo de 1,6 segundos.
5. Aplicar el índice compuesto validado en producción durante una ventana controlada por el posible bloqueo de tabla en MySQL 5.1.
6. Corregir permisos y flujo de procesamiento de la firma.
7. Eliminar la inclusión duplicada de `JSProgresBar.js`.
8. Localizar las excepciones del Grid activando ruptura al lanzar `ArgumentOutOfRangeException`.
9. A futuro, implementar paginación SQL real y evitar guardar 2.000 filas por sesión.

## Instrumentación temporal agregada

Los prefijos disponibles en la ventana Debug de Visual Studio son:

```text
WF_PARENT     Postback y selección en WebPrincipal
WF_PIPELINE   Etapas del pipeline ASP.NET
WF_SESSION    Adquisición y liberación de sesión por solicitud
WF_LIFECYCLE  Ciclo de vida del WebForm
WF_LOAD       Funciones internas de Page_Load
```

Esta instrumentación debe retirarse o quedar condicionada por configuración después de concluir el diagnóstico, especialmente `WF_SESSION`, porque puede producir gran volumen de salida.

## Detalle de la corrección de contención de sesión

La corrección debe aplicarse como un conjunto. Cambiar únicamente el intervalo del polling reduce la frecuencia, pero no elimina la serialización mientras todos los handlers pidan acceso exclusivo a `Session`.

### 1. Retirar la sesión obligatoria global

Actualmente `Global.asax.vb` fuerza sesión de lectura y escritura para todas las solicitudes:

```vb
Protected Sub Application_PostAuthenticateRequest(
    ByVal sender As Object,
    ByVal e As EventArgs)

    HttpContext.Current.SetSessionStateBehavior(
        System.Web.SessionState.SessionStateBehavior.Required)
End Sub
```

`Required` provoca un bloqueo exclusivo por `SessionID`. Mientras una solicitud lo conserva, cualquier otra solicitud del mismo usuario debe esperar.

Debe retirarse el comportamiento global:

```vb
Protected Sub Application_PostAuthenticateRequest(
    ByVal sender As Object,
    ByVal e As EventArgs)

    'No forzar SessionStateBehavior.Required globalmente.
End Sub
```

Esto no elimina automáticamente la sesión de las páginas Web Forms. Permite que cada handler declare explícitamente si necesita sesión exclusiva, de solo lectura o ninguna.

Antes de desplegar este cambio debe buscarse cualquier handler que utilice:

```text
HttpContext.Current.Session
context.Session
```

sin implementar `IRequiresSessionState` o `IReadOnlySessionState`.

### 2. Usar IReadOnlySessionState en handlers de solo lectura

Ejemplo actual:

```vb
Public Class Handler_lista_tareas_asignadas_workflow
    Implements IHttpHandler, IRequiresSessionState
```

Como ese handler solo lee un contador, debe utilizar:

```vb
Imports System.Web.SessionState

Public Class Handler_lista_tareas_asignadas_workflow
    Implements IHttpHandler, IReadOnlySessionState
```

Puede continuar leyendo:

```vb
context.Response.Write(
    context.Session("WF_NUMERO_TAREAS_SELECCIONADAS_W"))
```

`IReadOnlySessionState` permite lecturas concurrentes del mismo usuario. No debe utilizarse si el handler modifica valores de sesión.

### 3. No usar Session como variable de salida

Patrón actual:

```vb
ClassListandoTareas.Inicializa_lista_tarea_workflow_simple(
    Session("WF_NUMERO_TAREAS_SELECCIONADAS_W"))

context.Response.Write(
    Session("WF_NUMERO_TAREAS_SELECCIONADAS_W"))
```

El resultado temporal se escribe en sesión y obliga al acceso exclusivo. Debe reemplazarse por una variable local:

```vb
Dim numeroTareas As Integer = 0

Dim resultado As String =
    ClassListandoTareas.Inicializa_lista_tarea_workflow_simple(
        numeroTareas)

If resultado <> "YES" Then
    context.Response.StatusCode = 500
    context.Response.Write(resultado)
    Return
End If

context.Response.Write(numeroTareas)
```

Después de este cambio el handler puede implementar `IReadOnlySessionState` si las demás variables de sesión solamente se leen.

La misma revisión aplica a:

```text
GA_NUMERO_SOLICITUDES_PENDIENTES_APROBAR_USUARIO
GA_STRU_DOCUMENTO_PENDIENTE_REVISION
WF_NUMERO_TRAMITE_ASIGNADO
WF_NUMERO_TAREAS_SELECCIONADAS_W
```

Si estos valores únicamente se usan como respuesta AJAX, no deben persistirse en `Session`.

### 4. Sustituir el polling de 1,6 segundos

Código actual:

```javascript
INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS = setInterval(
    'remplaza_datos_lista_tareas_asignadas_workflow(...)',
    1600
);
```

Produce aproximadamente 37 solicitudes por minuto y usuario. Con 100 usuarios podría generar hasta 3.750 solicitudes por minuto.

Como cambio mínimo, debe aumentarse a 30 segundos y evitar la cadena evaluada:

```javascript
INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS = setInterval(
    function () {
        remplaza_datos_lista_tareas_asignadas_workflow(
            "",
            "Handler_lista_tareas_asignadas_workflow.ashx"
        );
    },
    30000
);
```

La alternativa recomendada es programar la siguiente consulta solo cuando termine la actual:

```javascript
function programarConsultaTareas() {
    consultarTareas(function () {
        INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS =
            setTimeout(programarConsultaTareas, 30000);
    });
}
```

Esto impide acumular peticiones cuando el servidor está lento. También debe evaluarse consolidar los distintos contadores en una sola petición AJAX.

### 5. Cancelar el polling al cambiar de pantalla

La medición demostró que el polling continúa mientras se carga `Webworkflow.aspx`.

Debe existir una función de limpieza:

```javascript
function detenerPollingInicio() {
    if (INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS) {
        clearInterval(INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS);
        clearTimeout(INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS);
        INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS = null;
    }
}
```

Debe ejecutarse al abandonar el documento:

```javascript
window.addEventListener("pagehide", detenerPollingInicio);
window.addEventListener("beforeunload", detenerPollingInicio);
```

Si la pantalla inicial permanece activa y solo cambia un iframe, debe invocarse explícitamente antes de asignar:

```javascript
iframe.src = "../workflow/Webworkflow.aspx";
```

### Orden seguro de implementación

1. Refactorizar los resultados temporales para que no se escriban en `Session`.
2. Cambiar handlers de lectura a `IReadOnlySessionState`.
3. Retirar el `SessionStateBehavior.Required` global.
4. Cambiar el `setInterval` por una ejecución controlada cada 30-60 segundos.
5. Cancelar timers al navegar.
6. Repetir las mediciones `WF_SESSION`.

El resultado esperado es que `Webworkflow.aspx` adquiera la sesión en pocos milisegundos, en vez de los 6.144 ms observados.

## Implementación realizada (4 de agosto de 2026)

- Se retiró de `Global.asax.vb` la asignación global de `SessionStateBehavior.Required`.
- Los diez handlers de contadores quedaron con `IReadOnlySessionState`.
- Los cinco handlers `_db` calculan el contador en una variable local y responden ese valor; ya no escriben resultados temporales en `Session`.
- `FileUploadHandler_` quedó marcado expresamente con `IRequiresSessionState`, porque este handler sí escribe datos en sesión y dependía del comportamiento global eliminado.
- Los cuatro contadores de la pantalla inicial consultan directamente sus handlers `_db` cada 30 segundos, en lugar de consultar valores de sesión cada 1,2 a 1,6 segundos.
- Se agregó `detener_polling_contadores_inicio()`. Se ejecuta en `pagehide` y antes de abrir un módulo en el iframe; al regresar a la pantalla inicial, los contadores se inicializan nuevamente.
- La solución compiló correctamente: 0 errores. Las advertencias reportadas por MSBuild son preexistentes.

Validación recomendada: repetir una navegación hacia Workflow y confirmar que `WF_PIPELINE|AcquireRequestState` y `WF_SESSION|ACQUIRED` ya no muestran esperas cercanas a 6-7 segundos.

### Ajuste posterior de caché

La primera validación continuó mostrando llamadas al handler antiguo cada 1,6 segundos. Esto confirmó que el navegador conservaba en caché la versión anterior de `WebFormInicioDocuarchiGestion.js`. Se agregó una versión a la URL del script (`?v=20260804-sessionfix1`) para forzar su actualización. También se eliminó la segunda inclusión de `JSProgresBar.js`, causante del error `Identifier 'JSProgresBar' has already been declared`.

## Acciones recomendadas para mejorar la carga en frío

La validación mostró que la primera carga después de iniciar, reciclar o recompilar la aplicación puede tardar entre 6 y 9 segundos mientras ASP.NET genera y carga ensamblados dinámicos `App_Web_*.dll`. La segunda carga dentro del mismo proceso IIS bajó a 377 ms. Esta demora inicial es independiente del bloqueo de sesión corregido.

### 1. Publicar la aplicación precompilada

Publicar el sitio utilizando la opción de precompilación de ASP.NET. Esto reduce el trabajo de compilación dinámica que normalmente se realiza durante la primera solicitud.

Antes de llevarlo a producción se debe validar:

- Compatibilidad de los Web Forms y archivos de recursos.
- Referencias a ensamblados del directorio `bin`.
- Controles personalizados y páginas compiladas dinámicamente.
- Que la publicación conserve archivos estáticos, handlers y configuraciones necesarias.

### 2. Mantener activo el Application Pool

En IIS, configurar el Application Pool correspondiente:

```text
Start Mode: AlwaysRunning
Idle Time-out (minutes): 0
Regular Time Interval (minutes): 0
```

`Idle Time-out = 0` evita que IIS detenga el proceso por inactividad. `Regular Time Interval = 0` desactiva el reciclaje periódico basado únicamente en tiempo. Deben conservarse los mecanismos de reciclaje necesarios por memoria, mantenimiento o despliegue según la política operativa.

### 3. Habilitar precarga de la aplicación

En la configuración avanzada de la aplicación IIS:

```text
Preload Enabled: True
```

También debe estar instalada y habilitada la característica `Application Initialization` de IIS.

### 4. Configurar Application Initialization

Agregar dentro de `system.webServer` en `web.config`, después de validar que no exista ya una sección equivalente:

```xml
<applicationInitialization doAppInitAfterRestart="true">
  <add initializationPage="/GestionDocumental-Docuarchi.net/gestor.aspx" />
</applicationInitialization>
```

La ruta debe ajustarse al nombre real de la aplicación configurada en IIS. Si el sitio está publicado en la raíz, la ruta deberá ser diferente.

### 5. No utilizar directamente Webworkflow.aspx como única página de calentamiento

`Webworkflow.aspx` depende de una sesión autenticada y de variables como usuario, ruta, grupo y actividad. Una solicitud anónima de inicialización no reproduce correctamente ese contexto y puede generar redirecciones o excepciones.

La opción más segura es crear un endpoint de calentamiento que:

- No requiera sesión de usuario.
- No escriba datos funcionales.
- Inicialice las dependencias y ensamblados principales.
- Abra y cierre conexiones de manera controlada, si se decide calentar el proveedor MySQL.
- Responda rápidamente con un estado simple.
- Esté restringido a IIS/local o protegido para evitar exposición pública innecesaria.

Después se configura ese endpoint como `initializationPage`.

### 6. Orden recomendado

1. Probar una publicación precompilada en ambiente local o de pruebas.
2. Habilitar `AlwaysRunning` y eliminar el apagado por inactividad.
3. Instalar `Application Initialization`.
4. Activar `Preload Enabled`.
5. Crear y probar un endpoint seguro de calentamiento.
6. Configurar ese endpoint como página de inicialización.
7. Reciclar el Application Pool.
8. Confirmar en los logs que el calentamiento ocurre antes del primer usuario.
9. Medir la primera navegación real y compararla con la segunda.

### 7. Resultado esperado

La combinación recomendada es:

```text
Publicación precompilada
+ Application Pool AlwaysRunning
+ Preload Enabled
+ Application Initialization
+ endpoint seguro de calentamiento
```

Esto no cambia el tiempo interno de `Page_Load`; busca que la compilación y carga inicial de ensamblados ocurra antes de que el primer usuario abra Workflow.
