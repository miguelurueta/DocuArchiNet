# DOC-119 — interfaz de seguimiento

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.

## Superficies UI

ImportacionCarpetasVista conserva el asistente. Resultados añade filtros de estado de carpeta y referencia de carpeta/efecto/recepción de archivos con rótulos accesibles, aplicar/limpiar y controles deshabilitados durante consulta. Aplicar reinicia la página del conjunto; los otros filtros se conservan. Enter en referencia aplica sin enviar el formulario WebForms.

La consulta invalida filas anteriores: si falla, no quedan datos/exportación obsoletos. CSV descarga una página con filtros/tamaño/página de la consulta representada.

La reorganización de accesos UI-01–05 sigue sin implementar; conserva compuerta de aceptación integrada SEG-01/02. No se retiró franja ni se cambió plugin contextual.

## Validacion visual

18/18 pruebas cliente en el último corte, Chromium local y servicios sintéticos. Los dos casos nuevos incluyen descarga Blob, filtros independientes, limpieza, paginación, denegación y viewport de 390 px sin desbordamiento. La prueba del formulario detecta envíos implícitos: Enter no causa submit. Esto no certifica render/autenticación IIS ni interfaz reorganizada.
