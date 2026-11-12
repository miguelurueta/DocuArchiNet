# DOC-119 — interfaz de seguimiento

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.

## Superficies UI

ImportacionCarpetasVista conserva el asistente. Resultados añade filtros de estado de carpeta y referencia de carpeta/efecto/recepción de archivos con rótulos accesibles, aplicar/limpiar y controles deshabilitados durante consulta. Aplicar reinicia la página del conjunto; los otros filtros se conservan. Enter en referencia aplica sin enviar el formulario WebForms.

La consulta invalida filas anteriores: si falla, no quedan datos/exportación obsoletos. CSV descarga una página con filtros/tamaño/página de la consulta representada.

UI-01–05 implementada después de la aceptación operativa: Importación ofrece importar/consultar al importador sin administrar perfiles, con administración separada. La consulta manual, último lote y verificación pendiente permanecen accesibles y los avisos visibles. Se retiró la franja permanente. El contextual captura el nivel pulsado; el menú superior conserva la selección. El asistente fija su destino y muestra la ruta autorizada por servidor.

## Validacion visual

99/99 pruebas de la batería final, con Chromium local y servicios sintéticos en los casos de navegador. Se revisó captura móvil390×640 de consulta. Se cubren reapertura/recarga/reinicialización y reemplazo de hosts, identidad, permisos denegados, referencia/último lote/confirmación incierta, filtros/CSV, viewport y foco. El plugin compartido tiene26 registros inventariados en22 archivos y prueba de contrato sobre cada selector; no se ejecutaron las operaciones de negocio reales de esos módulos. Compilación0errores/308advertencias previas. Detalle y huellas en `Validacion/doc119-ui-verificacion-20261112.md`; no certifica una nueva sesión autenticada de la UI final en IIS.
