# Guía quirúrgica para corregir la demora de carga de Workflow

Fecha de elaboración: 4 de agosto de 2026  
Aplicación: `GestionDocumental-Docuarchi.net`  
Página afectada: `workflow/Webworkflow.aspx`  
Tecnología: ASP.NET Web Forms, VB.NET, MySQL 5.1

## 1. Objetivo

Esta guía permite reproducir manualmente la corrección que redujo la segunda carga de Workflow de varios segundos a aproximadamente 377 ms.

La corrección resuelve dos problemas diferentes:

1. Todas las solicitudes dinámicas estaban forzadas a adquirir la sesión ASP.NET en modo exclusivo.
2. La pantalla inicial ejecutaba cuatro consultas AJAX cada 1,2 a 1,6 segundos, incluso mientras el usuario navegaba hacia Workflow.

El índice de MySQL mejora la consulta de tareas, pero no resuelve por sí solo la espera anterior al `Page_Load`.

## 2. Evidencia y diagnóstico confirmado

Antes de la corrección se observó:

```text
WF_PIPELINE|AcquireRequestState|6144 ms desde inicio request
WF_LIFECYCLE|Webworkflow.Page_Load entrada|6611 ms desde inicio request
WF_LOAD|TOTAL_PAGE_LOAD|299 ms
```

Interpretación:

- La solicitud esperaba aproximadamente 6 segundos antes de entrar al ciclo de vida de la página.
- El código real de `Page_Load` tardaba solamente entre 269 y 397 ms.
- El cuello de botella estaba en la adquisición de `Session`, no en la consulta principal del Workflow.

Después de la corrección, en una segunda carga dentro del mismo proceso IIS:

```text
WF_PIPELINE|AcquireRequestState|12 ms desde inicio request
WF_LIFECYCLE|Webworkflow.Page_Load entrada|25 ms desde inicio request
WF_LOAD|TOTAL_PAGE_LOAD|269 ms
WF_SESSION|END|...|377 ms
```

Resultado:

- Adquisición de sesión: de 6-8 segundos a 12 ms.
- Solicitud completa: 377 ms.
- La solución fue validada con 0 errores de compilación.

## 3. Respaldo y preparación

Antes de modificar producción:

1. Respaldar todos los archivos indicados en esta guía.
2. Aplicar primero en un ambiente local con una copia representativa de las tablas.
3. Detener cambios simultáneos sobre estos archivos.
4. Registrar los tiempos anteriores para poder comparar.
5. No eliminar código de sesión sin revisar si el componente escribe en ella.

Archivos involucrados en la corrección:

```text
Global.asax.vb
generic_control/FileUploadHandler_.ashx.vb
Defaul/WebFormInicioDocuarchiGestion.aspx
js/inicio/WebFormInicioDocuarchiGestion.js
workflow/Webworkflow.aspx
radicador/Handler_lista_numero_solicitudes.ashx.vb
radicador/Handler_lista_numero_solicitudes_dbase.ashx.vb
radicador/Handler_Lista_compartidos_por_revision.ashx.vb
radicador/Handler_Lista_compartidos_por_revision_db.ashx.vb
radicador/Handler_Lista_compartidos_para_otros_usuarios.ashx.vb
radicador/Handler_Lista_compartidos_para_otros_usuarios_db.ashx.vb
radicador/Handler_lista_tramites_wf_asignados.ashx.vb
radicador/Handler_lista_tramites_wf_asignados_db.ashx.vb
radicador/Handler_lista_tareas_asignadas_workflow.ashx.vb
radicador/Handler_lista_tareas_asignadas_workflow_db.ashx.vb
```

## 4. Cambio 1: retirar el bloqueo global de sesión

Archivo: `Global.asax.vb`

Localizar y eliminar únicamente este método:

```vb
Protected Sub Application_PostAuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
    HttpContext.Current.SetSessionStateBehavior(
        System.Web.SessionState.SessionStateBehavior.Required)
End Sub
```

### Por qué debe eliminarse

`SessionStateBehavior.Required` obliga a todas las solicitudes ASP.NET a usar sesión de lectura y escritura. ASP.NET aplica un bloqueo exclusivo por `SessionID`: mientras una solicitud mantiene el bloqueo, las demás solicitudes del mismo usuario quedan en espera.

Esto incluía páginas y handlers que no utilizaban sesión, handlers que solo leían valores y las llamadas AJAX frecuentes del tablero inicial.

### Precaución obligatoria

Después de eliminar la asignación global, cada handler que realmente necesite sesión debe declararlo explícitamente:

- Solo lectura: `IReadOnlySessionState`.
- Lectura y escritura: `IRequiresSessionState`.
- Sin uso de sesión: no implementar ninguna interfaz de sesión.

No cambiar indiscriminadamente todos los handlers a solo lectura. Un handler marcado como `IReadOnlySessionState` no debe modificar `Session`.

## 5. Cambio 2: preservar sesión exclusiva en el cargador de archivos

Archivo: `generic_control/FileUploadHandler_.ashx.vb`

Este handler utiliza y modifica numerosos valores de sesión. Antes dependía silenciosamente de la configuración global.

Cambiar:

```vb
Public Class FileUploadHandler_
    Implements System.Web.IHttpHandler
```

Por:

```vb
Public Class FileUploadHandler_
    Implements System.Web.IHttpHandler, System.Web.SessionState.IRequiresSessionState
```

Este paso debe realizarse antes o al mismo tiempo que se elimina `SessionStateBehavior.Required`; de lo contrario, la carga de archivos puede perder acceso a la sesión.

## 6. Cambio 3: convertir handlers de consulta a sesión de solo lectura

En los siguientes diez handlers, sustituir:

```vb
Implements System.Web.IHttpHandler, IRequiresSessionState
```

Por:

```vb
Implements System.Web.IHttpHandler, System.Web.SessionState.IReadOnlySessionState
```

Handlers:

```text
Handler_lista_numero_solicitudes.ashx.vb
Handler_lista_numero_solicitudes_dbase.ashx.vb
Handler_Lista_compartidos_por_revision.ashx.vb
Handler_Lista_compartidos_por_revision_db.ashx.vb
Handler_Lista_compartidos_para_otros_usuarios.ashx.vb
Handler_Lista_compartidos_para_otros_usuarios_db.ashx.vb
Handler_lista_tramites_wf_asignados.ashx.vb
Handler_lista_tramites_wf_asignados_db.ashx.vb
Handler_lista_tareas_asignadas_workflow.ashx.vb
Handler_lista_tareas_asignadas_workflow_db.ashx.vb
```

`IReadOnlySessionState` permite que varias consultas del mismo usuario lean la sesión de manera concurrente. Una solicitud que escriba en sesión seguirá siendo exclusiva.

## 7. Cambio 4: eliminar resultados temporales almacenados en Session

Los handlers `_db` necesitan leer identificadores del usuario, pero el contador calculado no necesita guardarse en sesión. Debe devolverse directamente en la respuesta HTTP.

### 7.1 Solicitudes pendientes de aprobación

Archivo: `Handler_lista_numero_solicitudes_dbase.ashx.vb`

Reemplazar el uso de la variable de sesión como parámetro `ByRef` por una variable local:

```vb
Dim numeroSolicitudes As Integer = 0
result = ref_clas.Retorna_numero_de_solicitudes_aprobacion_de_un_usuario(
    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
    numeroSolicitudes)
context.Response.Write(numeroSolicitudes)
```

Eliminar el uso de:

```vb
Session.Item("GA_NUMERO_SOLICITUDES_PENDIENTES_APROBAR_USUARIO")
```

como parámetro de salida y como valor de respuesta.

### 7.2 Documentos compartidos por revisión

Archivo: `Handler_Lista_compartidos_por_revision_db.ashx.vb`

Conservar:

```vb
Dim numero_documento As Integer = 0
```

Eliminar:

```vb
HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_PENDIENTE_REVISION") = numero_documento
```

Responder directamente:

```vb
context.Response.Write(numero_documento)
```

### 7.3 Documentos compartidos para otros usuarios

Archivo: `Handler_Lista_compartidos_para_otros_usuarios_db.ashx.vb`

Eliminar:

```vb
HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO") = numero_documento
```

Responder directamente:

```vb
context.Response.Write(numero_documento)
```

### 7.4 Trámites Workflow asignados

Archivo: `Handler_lista_tramites_wf_asignados_db.ashx.vb`

Usar la variable local existente como último parámetro:

```vb
Dim numero_documento As Integer = 0
result = ref_clas.Lista_numero_tramites(
    HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
    HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
    HttpContext.Current.Session.Item("Id_Grupo_Workflow"),
    HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"),
    HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
    numero_documento)
context.Response.Write(numero_documento)
```

No usar `WF_NUMERO_TRAMITE_ASIGNADO` como parámetro de salida.

### 7.5 Tareas Workflow asignadas

Archivo: `Handler_lista_tareas_asignadas_workflow_db.ashx.vb`

Implementar:

```vb
Dim Result As String = ""
Dim ClassListandoTareas As New ClassListandoTareas
Dim numeroTareas As Integer = 0
Result = ClassListandoTareas.Inicializa_lista_tarea_workflow_simple(numeroTareas)
context.Response.Write(numeroTareas)
```

No pasar `Session("WF_NUMERO_TAREAS_SELECCIONADAS_W")` como parámetro `ByRef`.

## 8. Cambio 5: reducir y cancelar el polling del tablero inicial

Archivo: `js/inicio/WebFormInicioDocuarchiGestion.js`

### 8.1 Definir intervalo único y función de limpieza

Después de las variables de intervalos, agregar:

```javascript
var INTERVALO_ACTUALIZACION_CONTADORES = 30000;

function detener_polling_contadores_inicio() {
    clearInterval(INTERVAL_REMPLAZA_PENDIETES_APROBACION);
    clearInterval(INTERVAL_REMPLADATOS_COMPARTIDOS);
    clearInterval(INTERVAL_REMPLAZA_LISTA_TRAMITES);
    clearInterval(INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS);
    INTERVAL_REMPLAZA_PENDIETES_APROBACION = null;
    INTERVAL_REMPLADATOS_COMPARTIDOS = null;
    INTERVAL_REMPLAZA_LISTA_TRAMITES = null;
    INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS = null;
}

window.addEventListener("pagehide", detener_polling_contadores_inicio);
```

### 8.2 Detener también los timers residuales de sesión y auditoría

`display_unload()` crea dos timers adicionales que no son contadores:

```javascript
INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR = setInterval(
    'Service_REST_validate_sesion_gestor();',
    '6030'
);

INTERVAL_LOG_SESION = setInterval(
    'set_actualiza_log_sesion_usuario_gestion_documental();',
    '31200'
);
```

El primero consulta `web_service_validate_sesion_active`, un método ASMX con
`EnableSession:=True`; el segundo actualiza el log de sesión. La función actual
`detener_polling_contadores_inicio()` solo cancela los cuatro contadores. Cuando
el usuario abre Workflow dentro del iframe, el documento padre no ejecuta
`pagehide`, por lo que ambos timers continúan ejecutándose durante la carga de
Workflow. Al volver al inicio, `display_unload()` los crea otra vez y pueden
quedar intervalos duplicados.

En `js/inicio/WebFormInicioDocuarchiGestion.js`, ampliar la función de limpieza
para que cancele los seis timers creados por `display_unload()`:

```javascript
function detener_polling_contadores_inicio() {
    clearInterval(INTERVAL_REMPLAZA_PENDIETES_APROBACION);
    clearInterval(INTERVAL_REMPLADATOS_COMPARTIDOS);
    clearInterval(INTERVAL_REMPLAZA_LISTA_TRAMITES);
    clearInterval(INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS);
    clearInterval(INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR);
    clearInterval(INTERVAL_LOG_SESION);

    INTERVAL_REMPLAZA_PENDIETES_APROBACION = null;
    INTERVAL_REMPLADATOS_COMPARTIDOS = null;
    INTERVAL_REMPLAZA_LISTA_TRAMITES = null;
    INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS = null;
    INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR = null;
    INTERVAL_LOG_SESION = null;
}
```

No se debe declarar otra variable local para
`INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR`: ya está declarada en
`js/sesion/js_sesion_gestor.js`. La referencia debe conservar exactamente ese
nombre.

`display_unload()` ya invoca esta función antes de crear los intervalos; esa
llamada debe conservarse. `event_menu_prinicipal()` también debe seguir
invocándola antes de abrir el módulo en el iframe.

Validación manual:

1. Abrir la pantalla inicial y confirmar una solicitud de validación cada
   ~6 segundos y una de log cada ~31 segundos.
2. Abrir Workflow y verificar, en Network, que después de las solicitudes que
   ya estuvieran en curso no se generen nuevas llamadas a
   `web_service_validate_sesion_active` ni a
   `Service_actualiza_log_sesion_usuario_gestion_documental` desde la pantalla
   inicial.
3. Regresar al inicio, esperar 40 segundos y confirmar que existe una sola
   secuencia de cada llamada, sin duplicados.
4. Repetir abrir Workflow y regresar al inicio al menos dos veces; la frecuencia
   no debe incrementarse en cada ciclo.

### 8.3 Evitar intervalos duplicados

Al comienzo de `display_unload()`, agregar:

```javascript
detener_polling_contadores_inicio();
```

Esto evita acumular timers si la pantalla inicial se vuelve a mostrar.

### 8.4 Sustituir los cuatro intervalos rápidos

Configurar los cuatro contadores con 30 segundos y llamar directamente al handler `_db`:

```javascript
INTERVAL_REMPLAZA_PENDIETES_APROBACION = setInterval(function () {
    remplaza_datos_respuestas_pendientes_por_aprobacion(
        "Respuestas pendientes por mi aprobación",
        "Handler_lista_numero_solicitudes_dbase.ashx");
}, INTERVALO_ACTUALIZACION_CONTADORES);

INTERVAL_REMPLADATOS_COMPARTIDOS = setInterval(function () {
    remplaza_datos_doucumentos_compartidos(
        "",
        "Handler_Lista_compartidos_por_revision_db.ashx");
}, INTERVALO_ACTUALIZACION_CONTADORES);

INTERVAL_REMPLAZA_LISTA_TRAMITES = setInterval(function () {
    remplaza_datos_lista_tramites_asignados(
        "",
        "Handler_lista_tramites_wf_asignados_db.ashx");
}, INTERVALO_ACTUALIZACION_CONTADORES);

INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS = setInterval(function () {
    remplaza_datos_lista_tareas_asignadas_workflow(
        "",
        "Handler_lista_tareas_asignadas_workflow_db.ashx");
}, INTERVALO_ACTUALIZACION_CONTADORES);
```

Antes existían intervalos de 1200, 1400, 1500 y 1600 ms. Con 100 usuarios producían miles de solicitudes por minuto.

### 8.5 Respetar el parámetro URL recibido

En las funciones de reemplazo, eliminar nombres de handler codificados internamente y utilizar `url_service`:

```javascript
web_service_lista_tramites_asignados(url_service);
web_service_solicitudes_documentos(url_service);
web_service_lista_tareas_asignadas_workflow(url_service);
```

La función de solicitudes pendientes ya utilizaba el parámetro correctamente.

### 8.6 Detener polling al abrir un módulo

Al inicio de `event_menu_prinicipal(element, event)`, agregar:

```javascript
detener_polling_contadores_inicio();
```

Cuando se regrese al tablero, al final de `even_diplay_ini()`, agregar:

```javascript
display_unload();
```

Esto reinicia los contadores de forma controlada y sin duplicarlos.

## 9. Cambio 6: invalidar la caché del JavaScript

Archivo: `Defaul/WebFormInicioDocuarchiGestion.aspx`

Cambiar:

```html
<script src="../js/inicio/WebFormInicioDocuarchiGestion.js"></script>
```

Por:

```html
<script src="../js/inicio/WebFormInicioDocuarchiGestion.js?v=20260804-sessionfix1"></script>
```

El valor de `v` puede cambiarse en cada publicación. Sin esta modificación, el navegador puede continuar ejecutando durante días el polling anterior aunque el archivo del servidor ya haya sido corregido.

Síntoma inequívoco de caché antigua:

```text
Handler_lista_tareas_asignadas_workflow.ashx
```

aparece repetidamente cada 1,6 segundos. Después de la corrección debe aparecer el handler `_db.ashx` aproximadamente cada 30 segundos.

## 10. Cambio 7: eliminar inclusión duplicada de JSProgresBar

Archivo: `workflow/Webworkflow.aspx`

`JSProgresBar.js` estaba incluido dos veces. Conservar la inclusión ubicada dentro de `<head>` y eliminar la segunda inclusión situada después de `</head>`.

La duplicación producía:

```text
Uncaught SyntaxError: Identifier 'JSProgresBar' has already been declared
```

Este error no originaba la espera de sesión, pero podía interrumpir la ejecución del JavaScript de Workflow.

## 11. Índice MySQL complementario

En `estados_tarea_workflow`, crear:

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

Antes del índice, producción utilizaba `index_merge`, examinaba aproximadamente 23.469 filas y ejecutaba `Using filesort`.

Después del índice, la prueba local utilizó `ref`, estimó aproximadamente 698 filas y eliminó el `filesort`.

El índice aumenta moderadamente el costo de `INSERT`, `UPDATE` y `DELETE`, pero evita combinar cuatro índices individuales y ordenar un conjunto grande para cada consulta de tareas pendientes.

## 12. Instrumentación opcional de diagnóstico

La instrumentación `WF_SESSION`, `WF_PIPELINE`, `WF_LIFECYCLE` y `WF_LOAD` fue agregada para localizar el retraso. No es indispensable para la corrección funcional.

Interpretación básica:

- `START`: inicio de la solicitud.
- `ACQUIRED`: sesión adquirida.
- `RELEASING`: la solicitud libera sesión.
- `END`: final de la solicitud.
- `AcquireRequestState`: momento en que ASP.NET intenta obtener el estado de sesión.
- `Page_Load entrada`: comienzo real del código de la página.
- `TOTAL_PAGE_LOAD`: duración total del método `Page_Load` medido.

Si `ACQUIRED` aparece varios segundos después de `START`, existe espera anterior al handler. Si `TOTAL_PAGE_LOAD` es bajo, no debe atribuirse la demora a la consulta del Workflow.

## 13. Compilación y publicación

Compilar la solución:

```powershell
msbuild.exe GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m
```

Resultado obtenido durante esta intervención:

```text
0 Errores
277 Advertencias
```

Las advertencias son preexistentes y deben revisarse por separado.

Después de publicar:

1. Reciclar el Application Pool una sola vez.
2. Abrir la pantalla inicial.
3. Forzar actualización del navegador con `Ctrl+F5`.
4. Abrir Workflow.
5. Repetir la navegación sin detener IIS para medir la carga caliente.

## 14. Diferenciar carga fría de bloqueo de sesión

La primera carga después de compilar, editar archivos o reiniciar IIS puede tardar varios segundos y mostrar mensajes como:

```text
App_Web_XXXXXXXX.dll cargado
```

Eso corresponde a compilación dinámica de ASP.NET. La comparación válida debe hacerse en el mismo proceso `w3wp`, sin editar archivos ni detener Visual Studio.

En la validación final:

- Primera carga en frío: aproximadamente 9,15 segundos.
- Segunda carga en el mismo `w3wp`: 377 ms.

Para reducir la primera carga en producción se recomienda publicar precompilado y configurar calentamiento del Application Pool. Esto es independiente de la corrección del bloqueo de sesión.

## 15. Lista de verificación funcional

Verificar después del cambio:

- El usuario puede iniciar sesión normalmente.
- El tablero muestra los cuatro contadores.
- Los contadores se actualizan cada 30 segundos.
- Al abrir Workflow dejan de generarse solicitudes del tablero.
- Al abrir Workflow dejan de generarse nuevas validaciones de sesión y
  actualizaciones de log originadas en la pantalla inicial.
- Tras volver al inicio varias veces, las llamadas de validación (~6 s) y de
  log (~31 s) no se duplican.
- Al regresar al inicio se reinician los contadores una sola vez.
- La carga de archivos conserva sus variables de sesión.
- Workflow lista las mismas tareas que antes.
- No aparece el error de redeclaración de `JSProgresBar`.
- En una carga caliente, `AcquireRequestState` permanece cercano a pocos milisegundos.

## 16. Problemas detectados que no forman parte de esta corrección

Estos errores continúan siendo independientes y requieren intervenciones separadas:

```text
System.ArgumentOutOfRangeException en System.Web.dll
System.UnauthorizedAccessException en tempfirma/REGISTROPUBLICO/<usuario>/<usuario>.bmp
System.ArgumentException en Handler_lista_tareas_asignadas_workflow_db.ashx
System.FormatException durante gestor.aspx
```

No deben confundirse con el bloqueo de sesión ya corregido.

## 17. Reversión manual

Si fuera necesario revertir:

1. Restaurar los archivos respaldados.
2. Restaurar los intervalos originales únicamente si se acepta nuevamente la carga elevada.
3. Restaurar `SessionStateBehavior.Required` solo junto con todos los handlers originales; hacerlo de forma aislada reintroduce el bloqueo global.
4. Si se elimina el índice:

```sql
ALTER TABLE estados_tarea_workflow
DROP INDEX IX_ETW_GRUPO_PENDIENTES;
```

5. Reciclar el Application Pool y validar inicio de sesión, tablero, Workflow y carga de archivos.

La reversión recomendada es restaurar el conjunto completo, no cambios individuales, porque la eliminación de resultados temporales de sesión y el uso directo de handlers `_db` están relacionados.

## 18. Criterio de éxito

La intervención se considera exitosa cuando, en una carga caliente y bajo el mismo proceso IIS:

```text
WF_PIPELINE|AcquireRequestState|< 100 ms
WF_LOAD|TOTAL_PAGE_LOAD|< 500 ms
WF_SESSION|END|...|< 1000 ms
```

La medición final obtenida fue:

```text
AcquireRequestState: 12 ms
Page_Load: 269 ms
Solicitud completa: 377 ms
```

## 19. Mejora complementaria de la carga en frío

La corrección de sesión mejora las cargas calientes. Para que el primer usuario tampoco asuma el costo de generar `App_Web_*.dll`, se recomienda aplicar conjuntamente:

```text
Publicación ASP.NET precompilada
Application Pool con Start Mode = AlwaysRunning
Idle Time-out = 0
Preload Enabled = True
Característica IIS Application Initialization habilitada
Endpoint seguro de calentamiento sin sesión de usuario
```

Configuración base en `web.config`, dentro de `system.webServer`:

```xml
<applicationInitialization doAppInitAfterRestart="true">
  <add initializationPage="/GestionDocumental-Docuarchi.net/gestor.aspx" />
</applicationInitialization>
```

La ruta debe ajustarse al despliegue real. No se recomienda usar `Webworkflow.aspx` como única página de calentamiento porque necesita una sesión autenticada y variables funcionales del usuario. La solución más segura es un endpoint dedicado, sin efectos secundarios y restringido a inicialización local.

Estas acciones deben probarse primero en un ambiente controlado. Desactivar el reciclaje periódico no implica eliminar controles por consumo de memoria ni los reciclajes coordinados durante publicaciones y mantenimiento.
