# Guía enterprise de despliegue estático: tablero, Workflow y expiración de sesión

Fecha: 2026-08-05  
Publicación preparada: `D:\temfile\Gestion`  
Tipo de publicación: ASP.NET Web Forms precompilada, actualizable (`updatable="true"`)  
Alcance: solo archivos estáticos (`.js` y `.aspx`); no requiere recompilación.

## 1. Propósito

Esta guía documenta de forma exacta los cambios aplicados a la publicación preparada para:

1. Reducir el polling excesivo de la pantalla inicial.
2. Evitar que los timers de la pantalla inicial se dupliquen al navegar mediante iframe.
3. Mostrar inmediatamente el número de tareas Workflow en las tarjetas.
4. Reemplazar el mensaje técnico `Object object` por un manejo controlado cuando expire la sesión en Workflow.

No documenta ni despliega cambios VB.NET, DLL, `Global.asax`, handlers compilados ni cambios MySQL.

## 2. Archivos modificados y creados

| Archivo | Tipo | Cambio |
|---|---|---|
| `js/inicio/WebFormInicioDocuarchiGestion.js` | Modificado | Timers, polling a 30 s y tarjeta Workflow. |
| `Defaul/WebFormInicioDocuarchiGestion.aspx` | Modificado | Versionado de JavaScript del tablero. |
| `js/workflow/session-workflow.js` | Nuevo | Manejo centralizado de expiración de sesión en Workflow. |
| `workflow/WebPrincipal.aspx` | Modificado | Referencia al manejador de sesión Workflow. |
| `workflow/Webworkflow.aspx` | Modificado | Referencia al manejador de sesión Workflow. |
| `workflow/WebFormGestionFlujoTrabajoCamaras.aspx` | Modificado | Referencias versionadas a los scripts Workflow. |
| `js/workflow/WebFormGestionFlujoTrabajoCamaras.js` | Modificado | Sustitución de alertas que mostraban objetos técnicos. |

## 3. Cambios del tablero inicial

### 3.1 Variables y función nueva de limpieza

Archivo: `js/inicio/WebFormInicioDocuarchiGestion.js`

Se agregó el intervalo único para contadores:

```javascript
var INTERVALO_ACTUALIZACION_CONTADORES = 30000;
```

Se agregó la función `detener_polling_contadores_inicio()`:

```javascript
function detener_polling_contadores_inicio() {
    clearInterval(INTERVAL_REMPLAZA_PENDIETES_APROBACION);
    clearInterval(INTERVAL_REMPLADATOS_COMPARTIDOS);
    clearInterval(INTERVAL_REMPLAZA_LISTA_TRAMITES);
    clearInterval(INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS);
    clearInterval(INTERVAL_SESION_ITEM_MANTENT);
    clearInterval(INTERVAL_LOG_SESION);

    INTERVAL_REMPLAZA_PENDIETES_APROBACION = null;
    INTERVAL_REMPLADATOS_COMPARTIDOS = null;
    INTERVAL_REMPLAZA_LISTA_TRAMITES = null;
    INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS = null;
    INTERVAL_SESION_ITEM_MANTENT = null;
    INTERVAL_LOG_SESION = null;
}

window.addEventListener("pagehide", detener_polling_contadores_inicio);
```

`display_unload()` invoca esta función antes de crear los timers. También se invoca desde `event_menu_prinicipal()` antes de navegar a un módulo y desde `even_diplay_ini()` al regresar al inicio.

### 3.2 Polling de cuatro contadores

Se sustituyeron los intervalos de 1,2; 1,4; 1,5 y 1,6 segundos por intervalos de 30 segundos:

```javascript
INTERVAL_REMPLAZA_PENDIETES_APROBACION = setInterval(function () {
    remplaza_datos_respuestas_pendientes_por_aprobacion(
        "Respuestas pendientes por mi aprobación",
        "Handler_lista_numero_solicitudes_dbase.ashx");
}, INTERVALO_ACTUALIZACION_CONTADORES);

INTERVAL_REMPLADATOS_COMPARTIDOS = setInterval(function () {
    remplaza_datos_doucumentos_compartidos(
        "", "Handler_Lista_compartidos_por_revision_db.ashx");
}, INTERVALO_ACTUALIZACION_CONTADORES);

INTERVAL_REMPLAZA_LISTA_TRAMITES = setInterval(function () {
    remplaza_datos_lista_tramites_asignados(
        "", "Handler_lista_tramites_wf_asignados_db.ashx");
}, INTERVALO_ACTUALIZACION_CONTADORES);

INTERVAL_REMPLAZA_DATOS_LISTA_TAREAS = setInterval(function () {
    remplaza_datos_lista_tareas_asignadas_workflow(
        "", "Handler_lista_tareas_asignadas_workflow_db.ashx");
}, INTERVALO_ACTUALIZACION_CONTADORES);
```

También se modificaron las funciones de trámites, documentos compartidos y tareas Workflow para respetar el parámetro `url_service`, en vez de usar internamente el handler anterior.

### 3.3 Función nueva: tarjeta de tareas Workflow

El error visual era que el DOM se actualizaba antes de terminar el AJAX, cuando `VALOR_TAREAS_ASIGNADAS` todavía valía `0`.

En el `success` de `web_service_lista_tareas_asignadas_workflow()` se agregó:

```javascript
VALOR_TAREAS_ASIGNADAS = data;
pintar_numero_tareas_asignadas_workflow(data);
```

La nueva función actualiza los tres badges y la tarjeta, únicamente cuando la respuesta ya está disponible:

```javascript
function pintar_numero_tareas_asignadas_workflow(numero_tareas) {
    if (!document.getElementById("id_task_workflow_")) return;

    var hayTareas = Number(numero_tareas) > 0;
    var visualizacion = hayTareas ? "block" : "none";

    document.getElementById("id_task_workflow").style.display = visualizacion;
    document.getElementById("id_task_workflow").textContent = numero_tareas;
    document.getElementById("id_task_workflow_").style.display = visualizacion;
    document.getElementById("id_task_workflow_").textContent = numero_tareas;
    document.getElementById("id_task_workflow__").style.display = visualizacion;
    document.getElementById("id_task_workflow__").textContent = numero_tareas;
}
```

La función `remplaza_datos_lista_tareas_asignadas_workflow()` queda limitada a iniciar la consulta; ya no pinta valores anteriores.

### 3.4 Invalidación de caché

Archivo: `Defaul/WebFormInicioDocuarchiGestion.aspx`

Referencia aplicada:

```html
<script src="../js/inicio/WebFormInicioDocuarchiGestion.js?v=20260805-sessionfix-pub1"></script>
```

El parámetro de versión evita que sea necesario borrar caché manualmente.

## 4. Manejo controlado de expiración de sesión en Workflow

### 4.1 Archivo nuevo

Archivo creado: `js/workflow/session-workflow.js`

Responsabilidades:

- Detectar respuestas HTTP 401/403.
- Detectar que una respuesta AJAX contiene la página `gestor.aspx`, usando los marcadores reales `TextBoxuser` e `INICIAR SESIÓN`.
- Evitar alertas que convierten objetos AJAX en `Object object`.
- Redirigir una sola vez al login mediante `window.top.location.href`.
- Registrar un manejador de postback asíncrono ASP.NET cuando `ScriptManager` ya esté disponible.

Código funcional principal:

```javascript
function respuestaEsLogin(xhr) {
    var texto = xhr.responseText || "";
    var url = xhr.responseURL || "";

    return xhr.status === 401 || xhr.status === 403 ||
        url.toLowerCase().indexOf("/gestor.aspx") >= 0 ||
        texto.indexOf('id="TextBoxuser"') >= 0 ||
        texto.indexOf("INICIAR SESIÓN") >= 0;
}

function redirigirPorSesion() {
    if (redireccionandoSesion) return;
    redireccionandoSesion = true;
    window.top.location.href = "../gestor.aspx";
}

window.mostrarErrorWorkflow = function (error) {
    if (respuestaEsLogin(error)) {
        redirigirPorSesion();
        return;
    }

    if (typeof error === "string") {
        alert(error);
        return;
    }

    alert(error.responseText || error.statusText ||
        "No fue posible completar la operación.");
};
```

### 4.2 Páginas que cargan el script nuevo

Se agregó la siguiente referencia, con versión, en:

- `workflow/WebPrincipal.aspx`
- `workflow/Webworkflow.aspx`
- `workflow/WebFormGestionFlujoTrabajoCamaras.aspx`

```html
<script src="../js/workflow/session-workflow.js?v=20260805-sessionfix1"></script>
```

### 4.3 Sustitución de alertas de objeto

Archivo: `js/workflow/WebFormGestionFlujoTrabajoCamaras.js`

Se reemplazaron dos ocurrencias de:

```javascript
alert(error);
```

por:

```javascript
mostrarErrorWorkflow(error);
```

La referencia de este archivo también quedó versionada:

```html
<script src="../js/workflow/WebFormGestionFlujoTrabajoCamaras.js?v=20260805-sessionfix1"></script>
```

## 5. Despliegue en una publicación precompilada

La publicación contiene:

```xml
<precompiledApp version="2" updatable="true"/>
```

Por esto los archivos estáticos de esta guía se pueden copiar individualmente. No se deben copiar individualmente archivos `.vb`, DLL ni `Global.asax.vb` desde el repositorio fuente.

Orden de despliegue recomendado:

1. Crear respaldo fechado de cada archivo existente a reemplazar.
2. Copiar los archivos JavaScript modificados.
3. Copiar `session-workflow.js` como archivo nuevo.
4. Copiar las páginas ASPX modificadas.
5. Validar sintaxis JavaScript con `node --check` sobre los JS modificados.
6. Abrir el navegador y confirmar en Network que se descargan las URLs con `?v=20260805-...`.
7. Validar funcionalmente según la sección 6.

## 6. Validación funcional

### Tablero

1. Abrir la pantalla inicial.
2. Confirmar que los cuatro contadores consultan inicialmente y luego cada 30 segundos.
3. Confirmar que la tarjeta Workflow muestra el número de tareas sin esperar el siguiente ciclo.
4. Abrir Workflow y confirmar que dejan de generarse los cuatro requests del tablero.
5. Regresar al inicio dos veces; confirmar que no se duplican los intervalos.

### Workflow y sesión

1. Abrir Workflow y ejecutar una acción asíncrona válida.
2. Forzar una sesión expirada en un ambiente de pruebas o esperar el timeout configurado.
3. Ejecutar una acción AJAX/postback asíncrono.
4. Confirmar redirección a `gestor.aspx` sin mensaje `Object object`.
5. Confirmar que el navegador descarga `session-workflow.js?v=20260805-sessionfix1`.

## 7. Reversión

Restaurar los respaldos creados antes del cambio.

En la publicación preparada se generaron dos grupos:

```text
*.20260805083841.bak  # tablero
*.20260805105442.bak  # Workflow
```

Para revertir Workflow, además de restaurar las tres páginas ASPX y
`WebFormGestionFlujoTrabajoCamaras.js`, eliminar `js/workflow/session-workflow.js` únicamente si se confirma que ninguna página lo sigue referenciando.

## 8. Límites y riesgos pendientes

1. El manejo de sesión del navegador es una mejora de experiencia; no evita el vencimiento cuando el navegador suspende una pestaña o hay pérdida prolongada de red.
2. El timeout de sesión actual es de 10 minutos. La continuidad real de sesión debe validarse por separado.
3. Los cambios backend de bloqueo de sesión siguen requiriendo una publicación compilada desde un baseline confiable.
4. Existe un defecto backend independiente: si `WF_RUTA_TEMPO_ESCANER` está vacío, la limpieza de temporales puede resolver la ruta como `C:\` e intentar borrar archivos del raíz. Debe corregirse en VB.NET con validación explícita de ruta antes de cualquier `Kill`.
5. La carpeta `Temp_Workflow` de la publicación activa debe otorgar permiso `Modify` a la identidad real del Application Pool; no se deben otorgar permisos sobre `C:\`.
